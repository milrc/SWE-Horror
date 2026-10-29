using Godot;
using System;

/// <summary>
/// A global node for keeping continuity between scene changes.
/// </summary>
public partial class Game : Node
{
    public static Game Instance {get; private set;}
    public override void _EnterTree()
    {
        Instance = this;
    }

}
