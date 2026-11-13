using Godot;
using System;

public partial class MainScene : Node3D
{
    private Mixer _mixer;

    public override void _Ready()
    {
        _mixer = GetNode<Mixer>("Mixer");
        _mixer.PlayTrack(SoundLibrary.Music.BEDROOM);
    }

}
