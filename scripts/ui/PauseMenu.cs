using Godot;

public partial class PauseMenu : Control
{
    [Export] Button ResumeButton;
    [Export] Button SettingsButton;
    [Export] Button QuitButton;
    private Vector2 LastMousePosition;

    // Set up the menu once Godot initializes nodes
    public override void _Ready()
    {
        ResumeButton.Pressed += () => Visible = false;
        SettingsButton.Pressed += () => {
            GameSettings.Instance.Visible = true;
            GameSettings.Instance.MoveToFront();
        };

        // Connect Quit button and assign lambda for closing the game to press event
        QuitButton.Pressed += () => GetTree().Quit();

        VisibilityChanged += () =>
        {
            if (Visible) {
                LastMousePosition = GetViewport().GetMousePosition();
                Engine.TimeScale = 0.0;
            } else {
                Engine.TimeScale = 1.0;
                GetViewport().WarpMouse(LastMousePosition);
            }
        };
    }
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (!GameSettings.Instance.Visible) 
                Visible = !Visible;
        }
    }
}
