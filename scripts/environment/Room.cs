using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

[Tool]
public partial class Room : GridAligned
{
    [Export] Texture2D Wall, Floor, Ceiling, Door;
    [Export] public Shader Shader; 
    [Export] public int Width_X, Width_Z;
    [Export] public bool Shaded = true;
    private bool _rebuild = false;
    [Export] public bool Rebuild {
        get => _rebuild;
        set {
            if (value) {
                RemoveSurfaces();
                DrawFloor();
            }
            _rebuild = false; 
        }
    }
    private bool _drawWalls = false;
    [Export] public bool DrawWalls {
        get => _drawWalls;
        set {
            if (value) {
                MarchFloors();
            }
            _drawWalls = false; 
        }
    }

    public void RemoveSurfaces()
    {
        //Remove old tiles
        foreach (Node child in GetChildren())
        {
            if (child is Surface)
                child.QueueFree();
        }
    }

    public void DrawFloor()
    {
        for (int x = 0; x < Width_X; x++)
        {
            for (int z = 0; z < Width_Z; z++)
            {
                Surface floor = new(){shaded = Shaded, shader = Shader};
                floor.RotateX(-Mathf.Pi/2f);
                floor.Position = new Vector3(x + 0.5f, 0, z + 0.5f);
                floor.SetMeta("_edit_group_", true);
                AddChild(floor);
                floor.Owner = GetTree().EditedSceneRoot;
                floor.Sprite.DoubleSided = false;
                floor.Sprite.Texture = Floor;
                floor.setShaderTexture(Floor, 1);
                floor.Name = "Floor";
            }
        }
    }
    public void MarchFloors()
    {
        //Collect which floors exist
        bool[,] floors = new bool[Width_X + 2, Width_Z + 2];
        foreach (Node3D child in GetChildren()) {
            if (child is not Surface) continue;
            floors[(int)(child.Position.X + 0.5f), (int)(child.Position.Z + 0.5f)] = true;
        }

        //Draw walls, ceilings where floors exist
        for (int x = 1; x <= Width_X; x++)
        {
            for (int z = 1; z <= Width_Z; z++)
            {
                //skip if no floor
                if (!floors[x,z]) continue;

                //Add ceiling
                Surface ceil = new(){shaded = Shaded, shader = Shader};
                ceil.RotateX(Mathf.Pi/2f);
                ceil.Position = new Vector3(x - 0.5f, 1f, z - 0.5f);
                ceil.SetMeta("_edit_group_", true);
                AddChild(ceil);
                ceil.Owner = GetTree().EditedSceneRoot;
                ceil.Sprite.DoubleSided = false;
                ceil.Sprite.Texture = Ceiling;
                ceil.setShaderTexture(Ceiling, 1);
                ceil.Name = "Ceiling";

                //Add walls
                Tuple<Vector2I, float, string>[] dirs = {
                    new(new Vector2I(0, 1), Mathf.Pi, "N"),
                    new(new Vector2I(0, -1), 0, "S"),
                    new(new Vector2I(-1, 0), Mathf.Pi/2f, "E"),
                    new(new Vector2I(1, 0), 3f*Mathf.Pi/2f, "W"),
                };
        
                foreach (var dir in dirs)
                {
                    if (!floors[dir.Item1.X + x,dir.Item1.Y + z]) {
                        Wall wall = new(){shaded = Shaded, shader = Shader};
                        wall.SetMeta("_edit_group_", true);
                        AddChild(wall);
                        wall.Owner = GetTree().EditedSceneRoot;
                        wall.RotateY(dir.Item2);
                        wall.Position = new Vector3(x - 0.5f, 0.5f, z - 0.5f) + new Vector3(dir.Item1.X/2f, 0, dir.Item1.Y/2f);
                        wall.Sprite.DoubleSided = false;
                        wall.Sprite.Texture = Wall;
                        wall.setShaderTexture(Wall, 1);
                        wall.Name = dir.Item3 + "Wall";
                    }
                }
            }
        }
    }
}
