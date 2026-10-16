using Godot;
using System;

[Tool]
public partial class Surface : GridAligned
{
    [Export] public ScaledSprite Sprite;
    [Export] public bool shaded;

    public override void _EnterTree()
    {
        if (Sprite == null)
        {
            Sprite = new();
            Sprite.Shaded = shaded;
            AddChild(Sprite);
            Sprite.Owner = GetTree().EditedSceneRoot;          
        }
        base._EnterTree();
    }
}
