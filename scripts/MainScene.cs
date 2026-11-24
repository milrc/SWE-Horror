using Godot;
using System;

public partial class MainScene : Node3D
{
    public override void _Ready()
    {
        Mixer.Instance.PlayTrack(SoundLibrary.Music.BEDROOM);
    }

}
