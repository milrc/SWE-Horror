using Godot;
using System;

[Tool]
public partial class Room : GridAligned
{
    [Export] Texture2D Wall, Floor, Ceiling, Door;
    [Export] public int Size;
    private bool _update = false;

    [Export] public bool Update {
        get => _update;
        set {
            if (value) DrawTiles();
            _update = false; 
        }
    }


    public void DrawTiles()
    {
        //Create new ones
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                Surface floor = new();
                floor.RotateX(Mathf.Pi/2f);
                floor.Position = new Vector3(x + 0.5f, 0, y + 0.5f);
                AddChild(floor);
                floor.Owner = GetTree().EditedSceneRoot;
                floor.Sprite.Texture = Wall;
            }
        }
    }
}
