using Godot;
using System;

public partial class FakeTunnel : Node3D
{
    [Export] VisibleOnScreenNotifier3D WrongSideVisible;
    public override void _Ready()
    {
        WrongSideVisible.ScreenEntered += () =>
        {
            RotateY(Mathf.Pi);
        };
    }
}
