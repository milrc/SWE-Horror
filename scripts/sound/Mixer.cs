using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// For global sounds and music
/// </summary>

public partial class Mixer : Node
{
    public AudioPool<AudioStreamPlayer> SoundPool = new();
    public Dictionary<SoundLibrary.Sound, List<AudioStreamPlayer>> EffectPlayers = new();
    public Dictionary<SoundLibrary.Sound, Gated> SoundTimers = new();
    public AudioStreamPlayer MusicPlayer;
    private AudioStreamPlayer _MusicFader;
    private bool _Transitioning = false;
    private double _timeElapsed = 0.0;
    private double _transitionTime = 1.0;
    public Queue<SoundLibrary.Sound> MusicQueue = new();
    static public Mixer Instance {get; private set;}
    
    public override void _EnterTree()
    {
        Instance = this;
    }
    public override void _Process(double delta)
    {
        //Handle ongoing transitions
        if (_Transitioning)
        {
            //mix out old music and mix in new music
            MusicPlayer.VolumeLinear = Mathf.Lerp(1f, 0f, (float)Math.Clamp(_timeElapsed / _transitionTime, 0.0, 1.0));
            _MusicFader.VolumeLinear = Mathf.Lerp(0f, 1f, (float)Math.Clamp(_timeElapsed / _transitionTime, 0.0, 1.0));
            _timeElapsed += delta;
        }
    }
    public void Pause()
    {
        MusicPlayer.StreamPaused = true;
    }
    public void Resume()
    {
        MusicPlayer.StreamPaused = false;
    }
    /// <summary>
    /// Play a track whenever available. Adds to a queue if a track is already playing
    /// </summary>
    /// <param name="track">
    /// The SoundLibrary.Music track to play.
    /// </param>
    /// <param name="skipCurrent">
    /// Whether or not the music player should respect the queue. Set to true to abruptly replace the current track, leaving the queue in place for when it finishes.
    /// See TransitionTo for the same behavior but smoother.
    /// </param>
    public void PlayTrack(SoundLibrary.Sound track, bool skipCurrent = false)
    {
        // Add a music player if it does not exist
        if (MusicPlayer == null) _CreateMusicPlayer();

        // Add the track to the queue if the player is busy and we are patient
        if (MusicPlayer.Playing && !skipCurrent) {
            MusicQueue.Enqueue(track);
        } else { //otherwise: play the track
            MusicPlayer.Stream = GD.Load<AudioStream>(track.GetVariant());
            MusicPlayer.Play();
        }
    }
    private void _CreateMusicPlayer()
    {
        MusicPlayer = new();
        AddChild(MusicPlayer);
        MusicPlayer.Finished += _PickTrackFromQueue;        
    }
    private void _PickTrackFromQueue() {
        if (MusicQueue.Count > 0) { //if there's more songs to play
            SoundLibrary.Sound nextTrack = MusicQueue.Dequeue();
            MusicPlayer.Stream = GD.Load<AudioStream>(nextTrack.GetVariant());
            MusicPlayer.Play();
        } else { //otherwise: loop last song
            MusicPlayer.Play();
        }
    }
    /// <summary>
    /// Replace the current track with a new one, mixing over a given time.
    /// </summary>
    /// <param name="track">
    /// The new track to play
    /// </param>
    /// <param name="overTime">
    /// The time (in seconds) that the transition will occur over
    /// </param>
    public void TransitionTo(SoundLibrary.Sound track, float overTime)
    {
        if (_MusicFader != null || _Transitioning) return; //Do not transition if one is already being processed

        //restart old track to fill air (if there is an old track)
        if (MusicPlayer != null && !MusicPlayer.Playing)
            MusicPlayer.Play();
        
        // Add a music player if it does not exist
        if (MusicPlayer == null) _CreateMusicPlayer();

        //start new track
        _MusicFader = new();
        AddChild(_MusicFader);
        _MusicFader.Stream = GD.Load<AudioStream>(track.GetVariant());
        _MusicFader.Play();
        
        //Begin transition
        _Transitioning = true;
        _timeElapsed = 0.0;
        _transitionTime = overTime;

        //create timer to mark end of transition
        Timer t = new()
        {
            OneShot = true,
            Autostart = true,
            WaitTime = overTime
        };
        AddChild(t);
        t.Timeout += () => {
            //replace old track with new
            MusicPlayer.QueueFree();
            MusicPlayer = _MusicFader;

            //end transition
            _Transitioning = false;
            MusicPlayer.VolumeLinear = 1f;

            //delete new
            _MusicFader = null;

            //remove timer
            t.QueueFree();
        };
    }
    /// <summary>
    ///     Ask the mixer to play a sound effect or song.
    /// </summary>
    /// <param name="sound">
    ///     The Sound struct to play. See SoundLibrary.
    /// </param>
    /// <param name="pitchEpsilon">
    ///     The range that the pitch scale can be randomly set. 
    ///     I.e., set to 1.0, the pitch scale can be 0.0 or 2.0 or anywhere in between.
    /// </param>
    public void Play(SoundLibrary.Sound sound, float pitchEpsilon = 0f)
    {
        AudioStreamPlayer p = new();
        AddChild(p);
        SoundPool.StartPlayer(EffectPlayers, p, sound, pitchEpsilon);
    }
    /// <summary>
    ///     Ask the mixer to play a sound effect or song, given that it's been enough time since it was last played
    /// </summary>
    /// <param name="timeLimit">
    ///     The minimum amount of time that needs to elapse before a sound effect can be played again.
    /// <\param>
    /// <param name="sound">
    ///     The Sound struct to play. See SoundLibrary.
    /// </param>
    /// <param name="pitchEpsilon">
    ///     The range that the pitch scale can be randomly set. 
    ///     I.e., set to 1.0, the pitch scale can be 0.0 or 2.0 or anywhere in between.
    /// </param>
    public void PlayGated(float timeLimit, SoundLibrary.Sound sound, float pitchEpsilon = 0f)
    {
        if (!SoundTimers.ContainsKey(sound))
        {
            SoundTimers[sound] = new(timeLimit);
        }
        if (SoundTimers[sound].Use())
        {
            SoundTimers[sound].SetTime(timeLimit);
            Play(sound, pitchEpsilon);
        }
    }
}