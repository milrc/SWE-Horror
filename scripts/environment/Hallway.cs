using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Hallway : Node3D
{
    public enum Directions {FORWARD, BACKWARD, TURN_LEFT, TURN_RIGHT};
    [Export] public Godot.Collections.Array<Directions> Password;
    [Export] public Godot.Collections.Array<Node3D> RemoveOnSolution;
    [Export] public Node3D Exit;
    Queue<Directions> inputQueue;

    public override void _Ready()
    {
        base._Ready();
        inputQueue = new();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // keyboard movement
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return;

        if (Input.IsActionJustPressed("move_forward")) {
            inputQueue.Enqueue(Directions.FORWARD);
        } else if (Input.IsActionJustPressed("move_backward")) {
            inputQueue.Enqueue(Directions.BACKWARD);
        } else if (Input.IsActionJustPressed("turn_left")) {
            inputQueue.Enqueue(Directions.TURN_LEFT);
        } else if (Input.IsActionJustPressed("turn_right")) {
            inputQueue.Enqueue(Directions.TURN_RIGHT);
        }

        if (inputQueue.Count > Password.Count) { //shed extra inputs
            inputQueue.Dequeue();
        }

        if (checkSolution())
            allowExit();
    }

    private bool checkSolution()
    {
        if (inputQueue.Count != Password.Count)
            return false;
        
        int i = 0;
        foreach (Directions dir in inputQueue)
        {
            if (dir != Password[i])
                return false;
            i++;
        }

        return true;
    }

    private void allowExit()
    {
        //Remove triggers and fake tunnel
        for (int i = 0; i < RemoveOnSolution.Count; i++) {
            RemoveOnSolution[i].QueueFree();
        }

        //Make exit visible
    }
}
