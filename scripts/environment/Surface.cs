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
        if (Sprite == null)
        {
            Sprite = new();
            Sprite.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
            ShadeSprite();
            AddChild(Sprite);
            Sprite.Owner = GetTree().EditedSceneRoot;          
        }
        base._EnterTree();
    }

    public void ShadeSprite()
    {
        Sprite.Shaded = shaded;
        if(shader != null && shaded) {
            material = new ShaderMaterial(){Shader = shader};
            Sprite.MaterialOverride = material;
            Sprite.SetShaderTexture();
        }
    }
    
}
