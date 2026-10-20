using Godot;

public partial class GridPlayer : Node3D
{
    //the current test room is 4x4 tiles, with tile centers at 0.5, 1.5, 2.5 and 3.5.
    [Export] public float StepSize = 1.0f;
    [Export] public float MinX = 0.5f;
    [Export] public float MaxX = 3.5f;
    [Export] public float MinZ = 0.5f;
    [Export] public float MaxZ = 3.5f;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return; //don't repeat actions just because a key is held down.

        switch (key.Keycode)
        {
            case Key.W:
                Move(-Transform.Basis.Z);
                break;
            case Key.S:
                Move(Transform.Basis.Z);
                break;
            case Key.A:
                RotateY(Mathf.Pi / 2.0f);
                break;
            case Key.D:
                RotateY(-Mathf.Pi / 2.0f);
                break;
        }
    }

    private void Move(Vector3 direction)
    {
        //snap movement to the world X/Z grid to avoid floating point drift
        // after repeated quarter turns.
        Vector3 step = new Vector3(
            Mathf.Round(direction.X),
            0.0f,
            Mathf.Round(direction.Z)
        ) * StepSize;

        Vector3 next = Position + step;

        //temporary guard for the four perimeter walls of the test room.
        // When gameplay rooms are added, replace this with physics checks
        // or a walkable-tile map, instead of extending these fixed bounds.
        if (next.X < MinX - 0.001f || next.X > MaxX + 0.001f ||
            next.Z < MinZ - 0.001f || next.Z > MaxZ + 0.001f)
            return;

        Position = new Vector3(next.X, Position.Y, next.Z);
    }
}
