
using Godot;

public partial class GridPlayer : Node3D
{
    [Export] public float StepSize = 1f;
    [Export] public float RayHeight = 0.5f;
    [Export] public uint CollisionMask = 1;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return;

        switch (key.Keycode)
        {
            case Key.W:
                Move(-Transform.Basis.Z);
                break;
            case Key.S:
                Move(Transform.Basis.Z);
                break;
            case Key.A:
                RotateY(Mathf.Pi / 2);
                break;
            case Key.D:
                RotateY(-Mathf.Pi / 2);
                break;
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 step = new Vector3(
            Mathf.Round(direction.X),
            0,
            Mathf.Round(direction.Z)
        ) * StepSize;

        //check if theres a wall
        Vector3 start = GlobalPosition + Vector3.Up * RayHeight;
        Vector3 end = start + step;

        var space = GetWorld3D().DirectSpaceState;
        var ray = PhysicsRayQueryParameters3D.Create(start, end, CollisionMask);
        var hit = space.IntersectRay(ray);

        if (hit.Count != 0)
            return;

        GlobalPosition += step;
    }
}
