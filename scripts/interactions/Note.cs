using Godot;

public partial class Note : StaticBody3D
{
    [Export] public string Title = "Title";
    [Export(PropertyHint.MultilineText)] public string Text = "If you hear knocking, don't open the door.";
    [Export] public Texture2D BackgroundTexture;

    public bool CanReadFrom(GridPlayer player)
    {
        Vector3 noteCell = GlobalPosition.Floor();
        Vector3 playerCell = player.GlobalPosition.Floor();
        return (noteCell.X == playerCell.X) && (noteCell.Z == playerCell.Z);
    }
}
