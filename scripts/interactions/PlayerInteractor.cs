using Godot;

//raycast on the camera, checks what the mouse is pointing at
public partial class PlayerInteractor : RayCast3D
{
	[Export] public Label PromptLabel;
	[Export] public Label HandLabel;
	[Export] public Node3D HandPoint;
	[Export] public float Reach = 1.0f;
	// [Export] public float DoorReach = 1.0f;
	[Export] public int PromptFontSize = 11;
	[Export] public NoteUI Reader;

	private Camera3D camera;
	private GridPlayer player;
	private CollectibleKey heldKey;
	private PlaceSpot lastSpot;

	public override void _Ready()
	{
		camera = GetParent() as Camera3D;
		player = camera.GetParent<GridPlayer>();
		Input.MouseMode = Input.MouseModeEnum.Visible;
		PromptLabel.AddThemeFontSizeOverride("font_size", PromptFontSize);
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
		Door door = GetCollider() as Door;
		CollectibleKey key = GetCollider() as CollectibleKey;
		PlaceSpot spot = GetCollider() as PlaceSpot;
		Note note = GetCollider() as Note;

		if (lastSpot != null)
			lastSpot.ShowHighlight(false);
		lastSpot = null;
		PromptLabel.Hide();

		if (Reader != null && Reader.IsOpen)
			return;

		if (door != null)
		{
			PromptLabel.Text = "Left click to " + (door.IsOpen ? "close" : "open") + " door";
			PromptLabel.Show();
		}
		else if (note != null && Reader != null && note.CanReadFrom(player))
		{
			PromptLabel.Text = "Left click to read note";
			PromptLabel.Show();
		}
		else if (heldKey == null && key != null)
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
		if (Reader != null && Reader.IsOpen)
			return;

		if (@event is not InputEventMouseButton mouseClick ||
			mouseClick.ButtonIndex != MouseButton.Left || !mouseClick.Pressed)
			return;

		CheckMouse();
		GD.Print(GetCollider());

		if (GetCollider() is Door door)
		{
			// check if the door needs a key. If it does, send the key if we have one
			if(!door.needsKey)
			{
				door.ToggleDoor();
			}

			if(heldKey != null && door.useKey(heldKey))
			{
				heldKey.QueueFree();
				heldKey = null;
				UpdateHandLabel();

				door.needsKey = false;
				door.ToggleDoor();
			}

			ShowPrompt();
			GetViewport().SetInputAsHandled();
			return;
		}

		if (GetCollider() is Note note && Reader != null && note.CanReadFrom(player))
		{
			Reader.Open(note);
			ShowPrompt();
			GetViewport().SetInputAsHandled();
			return;
		}

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
