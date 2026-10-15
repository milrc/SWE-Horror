using Godot;
using System;

[Tool]
public partial class Wall : Surface
{
    [Export] public StaticBody3D StaticBody;
    [Export] public CollisionShape3D CollisionShape;
    public override void _EnterTree()
    {
        if (StaticBody == null)
        {
            StaticBody = new();
            AddChild(StaticBody);
            StaticBody.Owner = GetTree().EditedSceneRoot;

            CollisionShape = new();
            StaticBody.AddChild(CollisionShape);
            CollisionShape.Owner = GetTree().EditedSceneRoot;

            BoxShape3D box = new() {
                Size = new Vector3(1, 1, 0.05f)
            };
            CollisionShape.Shape = box;
        }
        base._EnterTree();
    }
}
