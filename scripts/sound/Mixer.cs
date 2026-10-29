using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// For global sounds and music
/// </summary>

public partial class Mixer : Node
{
    public AudioPool<AudioStreamPlayer> SoundPool = new();
    public Dictionary<SoundLibrary.Sound, List<AudioStreamPlayer>> Players = new();
    public Dictionary<SoundLibrary.Sound, Gated> SoundTimers = new();
    static public Mixer Instance {get; private set;}
    
    public override void _EnterTree()
    {
        Instance = this;
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
        SoundPool.StartPlayer(Players, p, sound, pitchEpsilon);
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