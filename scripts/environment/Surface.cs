using Godot;
using System;

[Tool]
public partial class Surface : GridAligned
{
    [Export] public ScaledSprite Sprite;

    public void CreateSprite() 
    {
        Sprite = new();
        AddChild(Sprite);
        Sprite.Owner = GetTree().EditedSceneRoot;
    }

    public override void _EnterTree()
    {
        if (Sprite == null) CreateSprite();
        base._Ready();
    }
}
