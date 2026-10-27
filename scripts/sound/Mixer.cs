using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// For global sounds and music
/// </summary>

public partial class Mixer : Node
{
    static public Mixer Instance {get; private set;}
    public Dictionary<SoundLibrary.Sound, List<AudioStreamPlayer>> Players = new();
    
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
        //Create a new player
        AudioStreamPlayer player = new AudioStreamPlayer();
        AddChild(player);

        //Start sound
        player.Stream = GD.Load<AudioStream>(sound.GetVariant());
        if (pitchEpsilon != 0f)
        {   //randomly skew pitch if requested
            RandomNumberGenerator rng = new();
            player.PitchScale = rng.RandfRange(1f + pitchEpsilon, 1f - pitchEpsilon);
        }
        player.Play();

        //Save player
        if (!Players.ContainsKey(sound)) Players[sound] = new();
        Players[sound].Add(player);

        //Queue cull
        player.Finished += () => {_CullStream(sound);};
    }
    private void _CullStream(SoundLibrary.Sound sound)
    {
        //Check that we have this key
        if (!Players.ContainsKey(sound)) return;

        //check if any players are finished and remove them
        for (int i = 0; i < Players[sound].Count; i++)
        {
            if (Players[sound][i].Playing) continue; //skip useful players
            Players[sound][i].QueueFree(); //remove from tree
            Players[sound].RemoveAt(i); //remove from dict
        }

        //remove key if now unused
        if (Players[sound].Count == 0)
            Players.Remove(sound);
    }
}
