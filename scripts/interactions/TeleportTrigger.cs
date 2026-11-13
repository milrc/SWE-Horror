using Godot;
using System;

public partial class TeleportTrigger : Area3D
{
    [Export] public Node3D Target;
    public override void _Ready()
    {
        BodyEntered += (Node3D Body) =>
        {
            if (Body.Name == "PLAYER_BODY")
            {
                GD.Print("Moved player");
                //move player
                GridPlayer player = Body.GetParent() as GridPlayer;
                player.GlobalPosition = Target.GlobalPosition - (GlobalPosition - player.GlobalPosition);
                player.SetDesiredPosition(Target.GlobalPosition);
            }
        };
    }
}
