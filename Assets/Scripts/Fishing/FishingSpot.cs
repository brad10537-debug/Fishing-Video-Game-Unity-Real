using System.Collections.Generic;
using UnityEngine;

// Legacy data source for the early bobber prototype.
// The active Wizard Pond fishing system uses FishData + FishingSpotInteractable.
[System.Serializable]
public class FishDefinition
{
    public string fishName = "Bluegill";
    [Min(0.01f)] public float catchWeight = 1f;
    [Min(0.5f)] public float reelSeconds = 3f;
    [Range(0.1f, 3f)] public float reelDifficulty = 1f;
}

public class FishingSpot : MonoBehaviour
{
    [Header("Bite Timing")]
    [Min(0.1f)] public float minimumBiteDelay = 2f;
    [Min(0.1f)] public float maximumBiteDelay = 6f;

    [Header("Fish In This Spot")]
    public List<FishDefinition> availableFish = new List<FishDefinition>
    {
        new FishDefinition { fishName = "Bluegill", catchWeight = 3f, reelSeconds = 2.5f, reelDifficulty = 0.8f },
        new FishDefinition { fishName = "Bass", catchWeight = 1f, reelSeconds = 4f, reelDifficulty = 1.3f }
    };

    private void OnValidate()
    {
        if (maximumBiteDelay < minimumBiteDelay)
        {
            maximumBiteDelay = minimumBiteDelay;
        }
    }

    public float GetBiteDelay()
    {
        return Random.Range(minimumBiteDelay, maximumBiteDelay);
    }

    public FishDefinition ChooseFish()
    {
        if (availableFish == null || availableFish.Count == 0)
        {
            return new FishDefinition();
        }

        float totalWeight = 0f;
        foreach (FishDefinition fish in availableFish)
        {
            totalWeight += Mathf.Max(0.01f, fish.catchWeight);
        }

        float roll = Random.Range(0f, totalWeight);
        foreach (FishDefinition fish in availableFish)
        {
            roll -= Mathf.Max(0.01f, fish.catchWeight);
            if (roll <= 0f)
            {
                return fish;
            }
        }

        return availableFish[availableFish.Count - 1];
    }
}
