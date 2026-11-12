
using System;
using Godot;

public partial class GridPlayer : Node3D
{
    // camera node
    private Camera3D _camera;
    private Vector2 _screenResHalf;
    // sound node
    private Mixer3D _mixer;

    [Export] public float StepSize = 1f;
    [Export] public float RayHeight = 0.5f;
    [Export] public uint CollisionMask = 1;
    [Export] public float MovementWeight = .3f;
    [Export] public float RotationWeight = .2f;
    [Export(PropertyHint.Range, "0.0,1.0")] public float CameraLimit = 1/6f;
    [Export] public NoteUI Reader;

    // movement variables
    private Vector3 realPosition;
    private Transform3D centerRotation;
    private Transform3D mouseRotation;

    public override void _Ready()
    {
        // set resolution for mouse camera movement
        _screenResHalf = GetViewport().GetVisibleRect().Size;
        _screenResHalf.X /= 2;
        _screenResHalf.Y /= 2;

        // grab camera
        _camera = GetNode<Camera3D>("PlayerCamera");
        _camera.Position = new Vector3(0f, 0.5f, 0f);

        _mixer = GetNode<Mixer3D>("Mixer3D");

        realPosition = GlobalPosition;

        // GlobalPosition = realPosition;
        centerRotation = Transform;
        mouseRotation = Transform3D.Identity;
    }


    public override void _Process(double delta)
    {
        if (Reader != null && Reader.IsOpen)
            return;

        // animate the player
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

        _mixer.Play(SoundLibrary.Effects.STEP, 0.1f);
        realPosition += step;
    }

    public void SetDesiredPosition(Vector3 position)
    {
        realPosition = position;
    }

    private void RotateSelf(float rotation)
    {
        centerRotation.Basis = centerRotation.Basis.Rotated(Vector3.Up, rotation);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Reader != null && Reader.IsOpen)
            return;

        // keyboard movement
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return;

        if (Input.IsActionJustPressed("move_forward")) {
            Move(-Transform.Basis.Z);
        } else if (Input.IsActionJustPressed("move_backward")) {
            Move(Transform.Basis.Z);
        } else if (Input.IsActionJustPressed("turn_left")) {
            RotateSelf(Mathf.Pi / 2f);
        } else if (Input.IsActionJustPressed("turn_right")) {
            RotateSelf(-Mathf.Pi / 2f);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Reader != null && Reader.IsOpen)
            return;

        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            ApplyMouseRotation(eventMouseMotion.Position);
        }
    }

    public void ApplyMouseRotation(Vector2 mousePos)
    {
        mousePos -= new Vector2(_screenResHalf.X, _screenResHalf.Y);

        // calculate our mouse X Y rotational values
        float cameraRadianLimit = CameraLimit * (float)Math.PI;
        float mouseXValue = -mousePos.X/_screenResHalf.X * cameraRadianLimit;
        float mouseYValue = -mousePos.Y/_screenResHalf.Y * cameraRadianLimit;

        mouseRotation.Basis = new Basis(Vector3.Up, mouseXValue);
        mouseRotation.Basis *= new Basis(Vector3.Right, mouseYValue);

        Transform3D newCamera = _camera.Transform;
        newCamera.Basis = mouseRotation.Basis;
        _camera.Transform = newCamera;
    }

    public void AnimateMovement()
    {
        if (GlobalPosition != realPosition)
        {
            GlobalPosition = GlobalPosition.Lerp(realPosition, MovementWeight);
            if (GlobalPosition.DistanceSquaredTo(realPosition) < 0.0001f)
            {
                GlobalPosition = realPosition;
            }
        }
    }

    public void AnimateCamera()
    {
        Quaternion from = new(Transform.Basis);
        Quaternion dest = new(centerRotation.Basis);
        Quaternion rotLerp = from.Slerp(dest, RotationWeight);
        Transform3D nextTransform = Transform;
        nextTransform.Basis = new(rotLerp);
        Transform = nextTransform;
    }
}
