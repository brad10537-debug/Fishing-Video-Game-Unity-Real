using UnityEngine;

// Optional ScriptableObject fish definition.
// Use these when you want fish data as reusable project assets instead of editing every fishing spot by hand.
[CreateAssetMenu(fileName = "FishSpecies", menuName = "Wizard Pond/Fish Species")]
public class FishSpeciesAsset : ScriptableObject
{
    [Header("Core Fish Data")]
    public string fishName = "Pond Minnow";
    public FishRarity rarity = FishRarity.Common;
    [Min(0)] public int coinValue = 8;
    [Min(0)] public int xpReward = 5;
    [Range(0.5f, 5f)] public float difficulty = 0.8f;
    [Min(0.1f)] public float minimumWeight = 0.5f;
    [Min(0.1f)] public float maximumWeight = 2.5f;
    [TextArea] public string description = "A small magical pond fish.";

    [Header("Prototype Minigame")]
    public FishBehaviorType behaviorType = FishBehaviorType.Timing;
    public FishEncounterType encounterType = FishEncounterType.TimingWindow;
    [Range(0.5f, 3f)] public float speedModifier = 1f;
    [Min(1)] public int requiredSuccessScore = 2;
    [Min(0)] public int failureTolerance = 2;
    public string preferredHabitat = "Wizard Village Pond";

    public FishSpeciesData ToRuntimeData()
    {
        float minWeight = Mathf.Min(minimumWeight, maximumWeight);
        float maxWeight = Mathf.Max(minimumWeight, maximumWeight);
        int minCoinValue = Mathf.Max(0, Mathf.RoundToInt(coinValue * 0.85f));
        int maxCoinValue = Mathf.Max(minCoinValue, Mathf.RoundToInt(coinValue * 1.15f));
        int minXP = Mathf.Max(0, Mathf.RoundToInt(xpReward * 0.85f));
        int maxXP = Mathf.Max(minXP, Mathf.RoundToInt(xpReward * 1.15f));

        return new FishSpeciesData
        {
            speciesName = fishName,
            rarity = rarity,
            behaviorType = behaviorType,
            encounterType = encounterType,
            baseDifficulty = difficulty,
            speedModifier = speedModifier,
            requiredSuccessScore = requiredSuccessScore,
            failureTolerance = failureTolerance,
            preferredHabitat = preferredHabitat,
            flavorText = description,
            weightRange = new Vector2(minWeight, maxWeight),
            magicPowerRange = GetMagicPowerRange(rarity),
            sellValueRange = new Vector2Int(minCoinValue, maxCoinValue),
            xpRewardRange = new Vector2Int(minXP, maxXP)
        };
    }

    private Vector2Int GetMagicPowerRange(FishRarity fishRarity)
    {
        switch (fishRarity)
        {
            case FishRarity.Uncommon:
                return new Vector2Int(8, 22);
            case FishRarity.Rare:
                return new Vector2Int(20, 45);
            case FishRarity.Epic:
                return new Vector2Int(40, 75);
            case FishRarity.Legendary:
                return new Vector2Int(75, 130);
            case FishRarity.Mythic:
                return new Vector2Int(120, 220);
            default:
                return new Vector2Int(1, 12);
        }
    }
}
