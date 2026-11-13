using Godot;

//this goes on the key scene. The key ID will be used for doors later
public partial class CollectibleKey : StaticBody3D
{
    [Export] public string KeyId = "test_brass_key";
    [Export] public string DisplayName = "brass key";

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
        Position = new Vector3(0, 0.045f, 0);
        RotationDegrees = new Vector3(90, 0, 0);
        CollisionLayer = 2;
        oldSpot = spot;
    }
}
