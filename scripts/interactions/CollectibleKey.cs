using Godot;

//this goes on the key scene. The key ID will be used for doors later
public partial class CollectibleKey : StaticBody3D
{
    [Export] public string KeyId = "test_brass_key";
    [Export] public string DisplayName = "brass key";

    [Export] public Vector3 placePosition = new(0, 0.045f, 0);
    [Export] public Vector3 placeRotation = new(90, 0, 0);

    private PlaceSpot oldSpot;

    public void PickUp(Node3D hand)
    {
        if (oldSpot != null)
        {
            oldSpot.RemoveKey(this);
            oldSpot = null;
        }

        CollisionLayer = 0; //dont let the mouse hit the key while holding it
        Reparent(hand);
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
    }

    public void PutDown(PlaceSpot spot)
    {
        Reparent(spot);
        Position = placePosition;
        RotationDegrees = placeRotation;
        CollisionLayer = 2;
        oldSpot = spot;
    }
}
