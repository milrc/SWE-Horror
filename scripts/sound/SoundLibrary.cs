using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public static class SoundLibrary
{
    public static class Effects
    {
        public const string BUS = "Sound Effects";
        public static readonly Sound STEP = new([
        "res://sound/sfx/footsteps/step_0.wav",
        "res://sound/sfx/footsteps/step_1.wav",
        "res://sound/sfx/footsteps/step_2.wav",
        "res://sound/sfx/footsteps/step_3.wav",
        "res://sound/sfx/footsteps/step_4.wav"
        ]);
    }
    public static class Ambience
    {
        public const string BUS = "Ambiance";
        public static readonly Sound ROOM = new([            
            "res://sound/ambience/room_ambience_0.mp3",
            "res://sound/ambience/room_ambience_1.mp3",
            "res://sound/ambience/room_ambience_2.mp3",
        ]);
    }
    public static class Music
    {
        public const string BUS = "Music";
        public static readonly Sound TITLE = new("res://sound/music/3AM.mp3");
        public static readonly Sound RUN = new("res://sound/music/Scary Mansion.mp3");
        public static readonly Sound BEDROOM = new("res://sound/music/fortuna.mp3");
    }
    public readonly struct Sound
    {
        public readonly List<string> Variants;
        public Sound(string path)
        {
            Variants = [path];
        }
        public Sound(List<string> paths)
        {
            Variants = paths;
        }
        public string GetVariant()
        {
            RandomNumberGenerator rng = new();
            return Variants[rng.RandiRange(0, Variants.Count - 1)];
        }
    }
}
