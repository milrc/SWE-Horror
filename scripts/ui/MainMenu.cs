using Godot;

public partial class MainMenu : Control
{
    // Able to choose new game scene in Godot inspector
    [Export] public PackedScene NewGameScene { get; set; }

    // Set up the menu once Godot initializes nodes
    public override void _Ready()
    {
        // Connect New Game button and assign StartNewGame method to press event
        Button newGameButton = GetNode<Button>("Center/MenuPanel/NewGame");
        newGameButton.Pressed += StartNewGame;

        // Connect Quit button and assign lambda for closing the game to press event
        GetNode<Button>("Center/MenuPanel/Quit").Pressed += () => GetTree().Quit();

        // Select New Game for keyboard focus
        newGameButton.GrabFocus();
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
