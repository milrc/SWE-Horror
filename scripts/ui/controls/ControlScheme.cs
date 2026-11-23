using Godot;
using System;
using System.Collections.Generic;

public partial class ControlScheme : Control
{
    //Expose options in editor
    [Export] Godot.Collections.Dictionary<string, ControlChoice> Actions = new();

    public override void _EnterTree()
    {
        UpdateInputs();
    }

    public void UpdateInputs()
    {
        // Fill out dictionary and update control choices with defaults
        foreach (var (inputName, choice) in Actions) {
            Actions[inputName].InputName = inputName;
            if (!InputMap.HasAction(inputName)) {
                GD.Print(inputName + " not found in InputMap");
                continue;
            }
            Godot.Collections.Array<InputEvent> events = InputMap.ActionGetEvents(inputName);

            if (events.Count >= 1)
            {
                Actions[inputName].Primary.inputEvent = events[0];
                Actions[inputName].Primary.UpdateText();
                Actions[inputName].Primary.InputChanged += (_) => {UpdateInputMap(inputName);};
                
                if (events.Count >= 2)
                {
                    Actions[inputName].Secondary.inputEvent = events[1];
                    Actions[inputName].Secondary.UpdateText();
                    Actions[inputName].Secondary.InputChanged += (_) => {UpdateInputMap(inputName);};
                } else { //check for double choice
                    Actions[inputName].Secondary.inputEvent = events[0];
                    Actions[inputName].Secondary.UpdateText();
                    Actions[inputName].Secondary.InputChanged += (_) => {UpdateInputMap(inputName);};
                }
            }
        }
    }

    public void UpdateInputMap(string inputName)
    {
        InputMap.ActionEraseEvents(inputName);
        InputMap.ActionAddEvent(inputName, Actions[inputName].Primary.inputEvent);
        InputMap.ActionAddEvent(inputName, Actions[inputName].Secondary.inputEvent);
    }
}
