using Godot;
using System;

public partial class GameSettings : Control
{
    public const string CONFIG_PATH = "user://FUN_HOUSE_SETTINGS.cfg";
    [Export] public Button SaveButton;
    [Export] public Button Cancel;
    [Export] public ControlScheme Controls;
    [ExportGroup("Graphics")]
    [Export] public OptionButton WindowMode;
    [ExportGroup("Volume")]
    [Export] public Slider MasterVolume;
    [Export] public Slider MusicVolume;
    [Export] public Slider AmbienceVolume;
    [Export] public Slider EffectsVolume;
    static public GameSettings Instance {get; private set;}
    public override void _Ready()
    {
        Instance = this;
        Load();
        Apply();
        SaveButton.Pressed += () => {Save(); Apply(); Close();};

        Cancel.Pressed += () => {Load(); Close();};
    }

    public void Save()
    {
        ConfigFile config = new();
        config.SetValue("graphics", "window_mode", WindowMode.Selected);

        config.SetValue("volume", "master", MasterVolume.Value);
        config.SetValue("volume", "music", MusicVolume.Value);
        config.SetValue("volume", "ambience", AmbienceVolume.Value);
        config.SetValue("volume", "effects", EffectsVolume.Value);

        foreach (StringName action in InputMap.GetActions())
        {
            GD.Print(action);
            // Skip built-in UI actions
            if (action.ToString().StartsWith("ui_"))
                continue;

            Godot.Collections.Array<InputEvent> events = InputMap.ActionGetEvents(action);
            
            // ConfigFile natively serializes Godot Arrays of InputEvent Resources
            config.SetValue("controls", action, events);
        }
        config.Save(CONFIG_PATH);
    }
    public void Load()
    {
        ConfigFile config = new();
        Error err = config.Load(CONFIG_PATH);
        if (err != Error.Ok) return;
        
        if (config.HasSection("graphics"))
        {
            WindowMode.Selected = (int)config.GetValue("graphics", "window_mode");
        }

        if (config.HasSection("volume"))
        {
            MasterVolume.Value =    (int)config.GetValue("volume", "master");
            MusicVolume.Value =     (int)config.GetValue("volume", "music");
            AmbienceVolume.Value =  (int)config.GetValue("volume", "ambience");
            EffectsVolume.Value =   (int)config.GetValue("volume", "effects");
        }

        if (config.HasSection("controls"))
        {
            foreach (StringName action in InputMap.GetActions())
            {
                if (action.ToString().StartsWith("ui_")) continue;

                Godot.Collections.Array<InputEvent> savedEvents = config.GetValue("controls", action).AsGodotArray<InputEvent>();
                InputMap.ActionEraseEvents(action);
                foreach (InputEvent inputEvent in savedEvents)
                    InputMap.ActionAddEvent(action, inputEvent);
            }

            Controls.UpdateInputs();
        }
    }
    public void Apply()
    {
        //graphics
        DisplayServer.WindowSetMode(WindowMode.Selected == 0 ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);

        //volume
        AudioServer.SetBusVolumeLinear(0, (float)MasterVolume.Value/100f);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex(SoundLibrary.Music.BUS), (float)MusicVolume.Value/100f);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex(SoundLibrary.Ambience.BUS), (float)AmbienceVolume.Value/100f);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex(SoundLibrary.Effects.BUS), (float)EffectsVolume.Value/100f);
    }

    public void Close()
    {
        Visible = false;
    }
}
