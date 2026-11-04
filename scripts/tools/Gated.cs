using Godot;
using System;

/// <summary>
/// Class that manages a bool that resets itself when it returns true.
/// </summary>
public class Gated
{
    public Gated() {
        timeLimit = 1f;
    }
    public Gated(float time) {
        this.timeLimit = time;
    }

    private float timeLimit = 1f;
    private Timer timer = null;
    private static Node root = null;
    public bool Ready = true;

    /// <summary>
    /// Get the state of the bool and start the timer if it was true.
    /// </summary>
    /// <returns>
    /// The state of the bool
    /// </returns>
    public bool Use() {
		if (Ready) {
			Ready = false;
			countDown();
			return true;
		}
		return false;
    }
    /// <summary>
    /// Force the bool to be True again, ignoring the timer.
    /// </summary>
    public void Reset()
    {
        Ready = true;
        timer?.Stop();
    }
    /// <summary>
    /// Set the time limit on the cooldown, applied the next time it is called
    /// </summary>
    /// <param name="time">
    /// The wait time in seconds between when the function returns true
    /// </param>
    public void SetTime(float time)
    {
        timeLimit = time;
        //if timer exists: set its next wait time
        if (timer != null) 
            timer.WaitTime = time;
    }
    private void countDown() {
        if (timer == null) {
            timer = new() {
                OneShot = true,
                Autostart = true,
                WaitTime = timeLimit
            };
            timer.Timeout += () => Ready = true;
            if (root == null) {
                root = (Engine.GetMainLoop() as SceneTree).Root;
            }
            root.AddChild(timer);
        }
        timer.Start();
    }
}
