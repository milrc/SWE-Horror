using Godot;

//Put this on the door root. The door just swings around its hinge when you press E.
public partial class Door : Node3D
{
    [Export] public CollisionShape3D DoorCollision;
    [Export] public Sprite3D WallUnderDoor;
    [Export] public float OpenAngle = -90f;
    [Export] public float SwingTime = 0.7f;

    private bool isOpen = false;
    public bool IsOpen => isOpen;
    private bool isMoving = false;

    public override void _Ready()
    {
        if (DoorCollision == null)
            DoorCollision = GetNode<CollisionShape3D>("DoorPanel/DoorBody/DoorCollision");
        if (WallUnderDoor == null)
            WallUnderDoor = GetNode<Sprite3D>("../room-builder/NWall2/DoorwayWall");

        WallUnderDoor.Visible = true;
    }

    public void ToggleDoor()
    {
        if (isMoving)
            return;

        isMoving = true;
        bool opening = !isOpen;
        float endAngle = opening ? Mathf.DegToRad(OpenAngle) : 0f;

        Tween swing = CreateTween();
        swing.TweenProperty(this, "rotation:y", endAngle, SwingTime)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        swing.TweenCallback(Callable.From(() =>
        {
            isOpen = opening;
            isMoving = false;

            //The separate Area3D still lets the player aim at the door to close it.
            DoorCollision.Disabled = isOpen;
            WallUnderDoor.Visible = !isOpen;
        }));
    }
}
