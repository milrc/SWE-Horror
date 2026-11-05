using Godot;
using System;

public partial class FlashLight : SpotLight3D
{
    private Camera3D camera;
    public override void _Ready()
    {
        camera = GetParent() as Camera3D;
    }

    public override void _Process(double delta)
    {
        Vector2 mouse = GetViewport().GetMousePosition();
        LookAt(camera.ProjectPosition(mouse, 1f));
    }

}
