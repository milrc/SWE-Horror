using Godot;

//a place where the player can put a key down (table, shelf, etc)
public partial class PlaceSpot : StaticBody3D
{
    [Export] public MeshInstance3D HighlightMesh;

    private CollectibleKey keyHere;

    public override void _Ready()
    {
        ShowHighlight(false);
    }

    public bool IsEmpty()
    {
        return keyHere == null;
    }

    public void ShowHighlight(bool show)
    {
        if (HighlightMesh != null)
            HighlightMesh.Visible = show;
    }

    public bool PutKey(CollectibleKey key)
    {
        if (key == null || !IsEmpty())
            return false;

        keyHere = key;
        key.PutDown(this);
        ShowHighlight(false);
        return true;
    }

    public void RemoveKey(CollectibleKey key)
    {
        if (keyHere == key)
            keyHere = null;
    }
}
