using Godot;

public partial class NoteUI : Control
{
    public bool IsOpen => Visible;

    private PanelContainer paper;
    private StyleBox defaultPaperStyle;
    private Label title;
    private RichTextLabel text;
    private Button closeButton;
    private Vector2 mousePositionBeforeReading;

    public override void _Ready()
    {
        paper = GetNode<PanelContainer>("Paper");
        defaultPaperStyle = paper.GetThemeStylebox("panel");
        title = GetNode<Label>("Paper/Margin/Content/Header/Title");
        text = GetNode<RichTextLabel>("Paper/Margin/Content/Text");
        closeButton = GetNode<Button>("Paper/CloseArea/CloseButton");
        closeButton.Pressed += Close;
        Hide();
    }

    public void Open(Note note)
    {
        mousePositionBeforeReading = GetViewport().GetMousePosition();
        paper.AddThemeStyleboxOverride("panel", note.BackgroundTexture == null ? defaultPaperStyle : new StyleBoxTexture { Texture = note.BackgroundTexture });
        title.Text = note.Title;
        text.Text = note.Text;
        text.ScrollToLine(0);
        Show();
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        // Mouse look uses absolute cursor position; restore it to preserve the camera angle.
        GetViewport().WarpMouse(mousePositionBeforeReading);
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsOpen)
            return;

        bool closeClick = @event is InputEventMouseButton mouseClick &&
            mouseClick.ButtonIndex == MouseButton.Left && mouseClick.Pressed;
        bool closeKey = @event is InputEventKey key && key.Pressed && !key.Echo &&
            (key.Keycode == Key.Escape || key.Keycode == Key.Space);

        if (closeClick || closeKey)
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventKey &&
            (@event.IsActionPressed("move_forward") || @event.IsActionPressed("move_backward") ||
             @event.IsActionPressed("turn_left") || @event.IsActionPressed("turn_right")))
        {
            // Let the same keypress reach GridPlayer after closing the note
            Close();
        }
    }
}
