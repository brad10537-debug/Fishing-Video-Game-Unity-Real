using UnityEngine;

// Put this on the Player if you want the HUD to show the current village area.
// Area objects need a trigger collider and VillageAreaMarker.
public class PlayerAreaTracker : MonoBehaviour
{
    public PrototypeHud prototypeHud;
    public string fallbackAreaName = "Village";

    private void Awake()
    {
        if (prototypeHud == null)
        {
            prototypeHud = FindAnyObjectByType<PrototypeHud>();
        }
    }

    private void Start()
    {
        SetArea(fallbackAreaName);
    }

    private void OnTriggerEnter(Collider other)
    {
        VillageAreaMarker marker = other.GetComponentInParent<VillageAreaMarker>();
        if (marker != null)
        {
            SetArea(marker.areaDisplayName);
            if (FishingDifficultyManager.Instance != null)
                FishingDifficultyManager.Instance.zoneLevel = marker.zoneLevel;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        VillageAreaMarker marker = other.GetComponentInParent<VillageAreaMarker>();
        if (marker != null)
        {
            SetArea(marker.areaDisplayName);
            if (FishingDifficultyManager.Instance != null)
                FishingDifficultyManager.Instance.zoneLevel = marker.zoneLevel;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        VillageAreaMarker marker = other.GetComponentInParent<VillageAreaMarker>();
        if (marker != null)
        {
            SetArea(fallbackAreaName);
            if (FishingDifficultyManager.Instance != null)
                FishingDifficultyManager.Instance.zoneLevel = 1;
        }
    }

    private void SetArea(string areaName)
    {
        if (prototypeHud != null)
        {
            prototypeHud.SetCurrentArea(areaName);
        }
    }
}
