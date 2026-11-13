using Godot;
using System;

public partial class Mouse
{
	private CompressedTexture2D arrow = GD.Load<CompressedTexture2D>("res://art/mouse/mouse.png");

	// Called when the node enters the scene tree for the first time.
	public Mouse()
	{
		Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow);
	}
}
