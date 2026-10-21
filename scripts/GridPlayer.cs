
using Godot;

public partial class GridPlayer : Node3D
{
    [Export] public float StepSize = 1f;
    [Export] public float RayHeight = 0.5f;
    [Export] public uint CollisionMask = 1;
    [Export] public float rotationWeight = .2f;

    // movement variables
    private Vector3 realPosition;
    private Transform3D realRotation;

    public override void _Ready()
    {
        realPosition = new Vector3(1.5f, 0.0f, 2.5f);
        // GlobalPosition = realPosition;
        realRotation = Transform;
    }


    public override void _Process(double delta)
    {
        // animate the lerp
        AnimateMovement();
        AnimateCamera();
    }

    
    private void Move(Vector3 direction)
    {
        Vector3 step = new Vector3(
            Mathf.Round(direction.X),
            0,
            Mathf.Round(direction.Z)
        ) * StepSize;

        //check if theres a wall
        Vector3 start = realPosition + Vector3.Up * RayHeight;
        Vector3 end = start + step;

        var space = GetWorld3D().DirectSpaceState;
        var ray = PhysicsRayQueryParameters3D.Create(start, end, CollisionMask);
        var hit = space.IntersectRay(ray);

        if (hit.Count != 0)
            return;

        realPosition += step;
        // GlobalPosition = realPosition;
    }

    private void RotateSelf(float rotation)
    {
        realRotation.Basis = realRotation.Basis.Rotated(Vector3.Up, rotation);
    }

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
                RotateSelf(Mathf.Pi / 2);
                break;
            case Key.D:
                RotateSelf(-Mathf.Pi / 2);
                break;
        }
    }

    public void AnimateMovement()
    {
        GlobalPosition = GlobalPosition.Lerp(realPosition, 0.3f);
    }

    public void AnimateCamera()
    {
        // I'll fix this later, I'm just learning to use quaternions - betto
        Quaternion a = new(Transform.Basis);
        Quaternion b = new(realRotation.Basis);
        Quaternion C = a.Slerp(b, rotationWeight);
        Transform3D other = Transform;
        other.Basis = new(C);
        Transform = other;
    }
}
