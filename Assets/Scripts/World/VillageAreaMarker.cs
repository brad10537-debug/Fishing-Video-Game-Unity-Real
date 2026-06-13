using UnityEngine;

// Lightweight label for scene areas. This helps keep the lobby readable
// without needing a bigger scene-management system yet.
public class VillageAreaMarker : MonoBehaviour
{
    public VillageAreaType areaType = VillageAreaType.MainPond;
    public int zoneLevel = 1;
    public string areaDisplayName = "Main Pond";
    [TextArea] public string areaDescription = "A cozy magical pond full of strange fish.";
    public Color suggestedMaterialColor = Color.green;
}
