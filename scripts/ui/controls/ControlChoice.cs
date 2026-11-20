using Godot;
using System;

public partial class ControlChoice : Control
{
    public string InputName;
    [Export] public ControlButton Primary;
    [Export] public ControlButton Secondary;
}
