using Godot;
using System;

[Tool]
public partial class GridAligned : Node3D
{
    private bool _currentlySnapping = false;
    public override void _Ready()
    {
        SetNotifyLocalTransform(true);
    }

    public void SnapTransform()
    {
        _currentlySnapping = true;
        //snap position to nearest intersection   
        Position = (Position*2f).Round()/2f;
        Vector3 rotationIncrement = Vector3.One*Mathf.Pi*0.5f;
        Rotation = (Rotation / rotationIncrement).Round()*rotationIncrement;
        _currentlySnapping = false;
    }

    override public void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationLocalTransformChanged:
                if (!_currentlySnapping)
                    SnapTransform();
                break;
        }
    }   
}
