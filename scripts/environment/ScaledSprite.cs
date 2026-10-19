using Godot;
using System;

[Tool]
public partial class ScaledSprite : Sprite3D
{
    [Export] public float Width = 1f;
    [Export] public float roughness = 0.0f;
    [Export] public Texture2D normal;

    public override void _EnterTree()
    {
        TextureChanged += FixScale;
        TextureChanged += SetShaderTexture;
    }

    public void FixScale()
    {
        if(Texture == null) {return;}
        PixelSize = Width / Texture.GetSize().X;
    }

    // roughness must be between 1 and 0
    public void SetShaderTexture()
    {
        if(!Shaded || MaterialOverride == null) {return;}
        // might as well just set all shader properties
        (MaterialOverride as ShaderMaterial).SetShaderParameter("texture_albedo", Texture);
        (MaterialOverride as ShaderMaterial).SetShaderParameter("roughness", roughness);
        (MaterialOverride as ShaderMaterial).SetShaderParameter("normal_map", normal);
    }
}
