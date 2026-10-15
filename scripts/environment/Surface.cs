using Godot;
using System;

[Tool]
public partial class Surface : GridAligned
{
    [Export] public ScaledSprite Sprite;

    public override void _EnterTree()
    {
        if (Sprite == null)
        {
            Sprite = new();
            AddChild(Sprite);
            Sprite.Owner = GetTree().EditedSceneRoot;            
        }
        base._EnterTree();
    }
}
