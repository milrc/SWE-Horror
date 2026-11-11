using Godot;

public partial class NoteUI : Control
{
    public bool IsOpen => Visible;

    private Label title;
    private RichTextLabel text;
    private Button closeButton;
    private Vector2 mousePositionBeforeReading;

    public override void _Ready()
    {
        title = GetNode<Label>("Paper/Margin/Content/Header/Title");
        text = GetNode<RichTextLabel>("Paper/Margin/Content/Text");
        closeButton = GetNode<Button>("Paper/CloseArea/CloseButton");
        closeButton.Pressed += Close;
        Hide();
    }

    public void Open(Note note)
    {
        mousePositionBeforeReading = GetViewport().GetMousePosition();
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
    }
}
