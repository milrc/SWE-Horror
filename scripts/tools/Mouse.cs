using Godot;
using System;

public partial class Mouse
{
	private readonly CompressedTexture2D arrow = GD.Load<CompressedTexture2D>("res://art/mouse/mouse.png");
	private readonly CompressedTexture2D hand = GD.Load<CompressedTexture2D>("res://art/mouse/mouse_hand.png");
	private readonly CompressedTexture2D question = GD.Load<CompressedTexture2D>("res://art/mouse/question_mark.png");

	public enum MouseMode {ARROW, HAND, QUESTION};
	// Called when the node enters the scene tree for the first time.
	public Mouse()
	{
		Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow);
		// Input.SetCustomMouseCursor(hand, Input.CursorShape.PointingHand);
		// Input.SetCustomMouseCursor(question, Input.CursorShape.Help);
	}

	public void SetMouse(MouseMode mode)
	{
		switch (mode)
		{
			case MouseMode.ARROW:
				Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow);
				break;
			case MouseMode.HAND:
				Input.SetCustomMouseCursor(hand, Input.CursorShape.Arrow);
				break;
			case MouseMode.QUESTION:
				Input.SetCustomMouseCursor(question, Input.CursorShape.Arrow);
				break;
			default:
				break;
		}
	}
}
