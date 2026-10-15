using Godot;
using System;

[Tool]
public partial class ScaledSprite : Sprite3D
{
    [Export] public float Width = 1f;
    public override void _EnterTree()
    {
        TextureChanged += FixScale;
    }

    public void FixScale()
    {
        PixelSize = Width / Texture.GetSize().X;
    }
}
