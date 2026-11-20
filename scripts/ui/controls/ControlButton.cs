using Godot;
using System;

public partial class ControlButton : Button
{
    public InputEvent inputEvent;
    public event Action<InputEvent> InputChanged;

    bool readingInput = false;
    public override void _Pressed()
    {
        if (readingInput) return;
        Text = "...";
        readingInput = true;
    }
    public override void _Input(InputEvent @event)
    {
        if (!readingInput) return;
        base._Input(@event);

        //ignore mouse movement
        if (@event is InputEventKey keyEvent && keyEvent.Pressed || @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (Input.IsActionJustPressed("ui_cancel")) //cancel rebind
            {
                readingInput = false;
                UpdateText();
                return;
            }
            SetInput(@event);

            // Prevent the input from triggering actions
            GetViewport().SetInputAsHandled();
        }
    }
    public void SetInput(InputEvent _inputEvent)
    {
        readingInput = false;
        inputEvent = _inputEvent;
        InputChanged?.Invoke(inputEvent);
        UpdateText();
    }
    public void UpdateText()
    {
        Text = inputEvent.AsText();

        // Remove physical text
        if (Text.Contains(" - Physical")) Text = Text[0..^11];

        // Cut off long text
        if (Text.Length > 14)
        {
            Text = Text[..11];
            if (Text[^1] == ' ')
                Text = Text[..10];
            Text += "...";
        }
    }
}
