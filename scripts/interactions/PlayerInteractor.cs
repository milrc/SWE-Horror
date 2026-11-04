using Godot;

//raycast on the camera, checks what the mouse is pointing at
public partial class PlayerInteractor : RayCast3D
{
    [Export] public Label PromptLabel;
    [Export] public Label HandLabel;
    [Export] public Node3D HandPoint;
    [Export] public float Reach = 2.8f;

    private Camera3D camera;
    private CollectibleKey heldKey;
    private PlaceSpot lastSpot;

    public override void _Ready()
    {
        camera = GetParent() as Camera3D;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        PromptLabel.Hide();
        UpdateHandLabel();
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckMouse();
        ShowPrompt();
    }

    private void CheckMouse()
    {
        //raay needs to follow the cursor instead of staying in the center
        Vector2 mouse = GetViewport().GetMousePosition();
        Vector3 start = camera.ProjectRayOrigin(mouse);
        Vector3 dir = camera.ProjectRayNormal(mouse);
        TargetPosition = ToLocal(start + dir * Reach);
        ForceRaycastUpdate();
    }

    private void ShowPrompt()
    {
        CollectibleKey key = GetCollider() as CollectibleKey;
        PlaceSpot spot = GetCollider() as PlaceSpot;

        if (lastSpot != null)
            lastSpot.ShowHighlight(false);
        lastSpot = null;
        PromptLabel.Hide();

        if (heldKey == null && key != null)
        {
            PromptLabel.Text = "Left click to pick up " + key.DisplayName;
            PromptLabel.Show();
        }
        else if (heldKey != null && spot != null && spot.IsEmpty())
        {
            spot.ShowHighlight(true);
            lastSpot = spot;
            PromptLabel.Text = "Left click to place " + heldKey.DisplayName;
            PromptLabel.Show();
        }

        PromptLabel.Position = GetViewport().GetMousePosition() + new Vector2(14, 18);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseClick ||
            mouseClick.ButtonIndex != MouseButton.Left || !mouseClick.Pressed)
            return;

        CheckMouse();

        if (heldKey == null)
        {
            CollectibleKey key = GetCollider() as CollectibleKey;
            if (key == null)
                return;

            heldKey = key;
            key.PickUp(HandPoint);
        }
        else
        {
            PlaceSpot spot = GetCollider() as PlaceSpot;
            if (spot == null || !spot.PutKey(heldKey))
                return;

            heldKey = null;
        }

        UpdateHandLabel();
        ShowPrompt();
        GetViewport().SetInputAsHandled();
    }

    private void UpdateHandLabel()
    {
        HandLabel.Text = heldKey == null ? "Hand: empty" : "Holding: " + heldKey.DisplayName;
    }
}
