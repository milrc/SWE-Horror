using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class Surface : GridAligned
{
    [Export] public ScaledSprite Sprite;
    [Export] public bool shaded;
    [Export] public Shader shader;

    private ShaderMaterial material;

    public override void _EnterTree()
    {
        if(shader != null) {
            material = new ShaderMaterial();
            material.Shader = shader;
        }

        if (Sprite == null)
        {
            Sprite = new();
            Sprite.Shaded = shaded;
            Sprite.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
            if(material != null) {Sprite.MaterialOverride = material;}
            AddChild(Sprite);
            Sprite.Owner = GetTree().EditedSceneRoot;          
        }
        base._EnterTree();
    }

    // roughness must be between 1 and 0
    public void setShaderTexture(Texture tex, float roughness)
    {
        // might as well just set all shader properties
        (Sprite.MaterialOverride as ShaderMaterial).SetShaderParameter("texture_albedo", tex);
        (Sprite.MaterialOverride as ShaderMaterial).SetShaderParameter("roughness", roughness);
        (Sprite.MaterialOverride as ShaderMaterial).SetShaderParameter("uv1_scale", new Vector3(1, 1, 1));
    }
}
