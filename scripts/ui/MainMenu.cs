using Godot;

public partial class MainMenu : Control
{
    // Able to choose new game scene in Godot inspector
    [Export] public PackedScene NewGameScene { get; set; }
    [Export] Button NewGameButton;
    [Export] Button SettingsButton;
    [Export] Button QuitButton;

    // Set up the menu once Godot initializes nodes
    public override void _Ready()
    {
        // Connect New Game button and assign StartNewGame method to press event
        NewGameButton.Pressed += StartNewGame;

        SettingsButton.Pressed += () => {
            GameSettings.Instance.Visible = true;
            GameSettings.Instance.MoveToFront();
        };

        // Connect Quit button and assign lambda for closing the game to press event
        QuitButton.Pressed += () => GetTree().Quit();
    }

    private void StartNewGame()
    {
        // Exit method if no NewGameScene
        if (NewGameScene == null)
        {
            GD.PushError("No new game scene assigned in MainMenu node");
            return;
        }

        // Change main menu scene to new game scene
        GetTree().ChangeSceneToPacked(NewGameScene);
    }
}
