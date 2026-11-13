using Godot;
using System;
using System.Collections.Generic;

public class AudioPool<T> where T : Node, new()
{
    public T StartPlayer(Dictionary<SoundLibrary.Sound, List<T>> Players, T player, SoundLibrary.Sound sound, float pitchEpsilon = 0f)
    {
        //Start sound
        player.Set(AudioStreamPlayer.PropertyName.Stream, GD.Load<AudioStream>(sound.GetVariant()));
        if (pitchEpsilon != 0f)
        {   //randomly skew pitch if requested
            RandomNumberGenerator rng = new();
            player.Set(AudioStreamPlayer.PropertyName.PitchScale, rng.RandfRange(1f + pitchEpsilon, 1f - pitchEpsilon));
        }
        player.Call(AudioStreamPlayer.MethodName.Play);

        //Save player
        if (!Players.ContainsKey(sound)) Players[sound] = new();
        Players[sound].Add(player);

        //Queue cull
        player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(() => _CullStream(Players, sound)));
        
        return player;
    }
    private void _CullStream(Dictionary<SoundLibrary.Sound, List<T>> Players, SoundLibrary.Sound sound)
    {
        //Check that we have this key
        if (!Players.ContainsKey(sound)) return;

        //check if any players are finished and remove them
        for (int i = 0; i < Players[sound].Count; i++)
        {
            if ((bool)Players[sound][i].Get(AudioStreamPlayer.PropertyName.Playing)) continue; //skip useful players
            Players[sound][i].QueueFree(); //remove from tree
            Players[sound].RemoveAt(i); //remove from dict
        }

        //remove key if now unused
        if (Players[sound].Count == 0)
            Players.Remove(sound);
    }
}