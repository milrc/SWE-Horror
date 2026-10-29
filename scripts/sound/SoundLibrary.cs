using Godot;
using System;
using System.Collections.Generic;

public static class SoundLibrary
{
    public static class Effects
    {
        public static readonly Sound STEP = new("res://sound/jump.wav");
    }
    public static class Music
    {
        public static readonly Sound TITLE = new("res://sound/title.mp3");
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
