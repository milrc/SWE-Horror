using Godot;
using System;

public partial class FlickeringLight : GridAligned
{
    [Export] public Node3D OnModel;
    [Export] public Node3D OffModel;
    [Export] public int Seed = -1;
    [Export] public float OnInterval, OffInterval, IntervalEpsilon;
    Timer _onT, _offT;
    RandomNumberGenerator rng;
    public override void _Ready()
    {
        //Seed RNG
        rng = new();
        if (Seed >= 0)
            rng.Seed = (ulong)Seed;

        //set up timers
        _onT = new()
        {
            Autostart = true,
            OneShot = true
        };
        AddChild(_onT);
        _onT.Timeout += () => {
            //when time out: turn off and start off timer
            OnModel.Visible = false;
            OffModel.Visible = true;
            startTimer(_offT, OffInterval, IntervalEpsilon);
        };

        _offT = new()
        {
            Autostart = true,
            OneShot = true
        };
        AddChild(_offT);
        _offT.Timeout += () =>
        {
            //when time out: turn on and start on timer
            OnModel.Visible = true;
            OffModel.Visible = false;
            startTimer(_onT, OnInterval, IntervalEpsilon);
        };

        //start on timer
        startTimer(_onT, OnInterval, IntervalEpsilon);
    }
    void startTimer(Timer t, float waitTime, float epsilon)
    {
        t.WaitTime = rng.RandfRange(waitTime * (1f - epsilon), waitTime * (1f + epsilon));
        t.Start();
    }
}
