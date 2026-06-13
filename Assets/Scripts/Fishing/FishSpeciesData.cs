using UnityEngine;

[System.Serializable]
public class FishSpeciesData
{
    public string speciesName = "Moonfin";
    public FishRarity rarity = FishRarity.Common;
    public FishBehaviorType behaviorType = FishBehaviorType.Timing;
    public FishEncounterType encounterType = FishEncounterType.TimingWindow;
    [Range(0.1f, 5f)] public float baseDifficulty = 1f;
    [Range(0.5f, 3f)] public float speedModifier = 1f;
    [Min(1)] public int requiredSuccessScore = 3;
    [Min(0)] public int failureTolerance = 2;
    public string preferredHabitat = "Village Pond";
    [TextArea] public string flavorText = "A magical pond fish with a little attitude.";
    public Vector2 weightRange = new Vector2(1f, 5f);
    public Vector2Int magicPowerRange = new Vector2Int(1, 15);
    public Vector2Int sellValueRange = new Vector2Int(5, 25);
    public Vector2Int xpRewardRange = new Vector2Int(4, 10);

    public int GetAverageCoinValue()
    {
        return Mathf.RoundToInt((sellValueRange.x + sellValueRange.y) * 0.5f);
    }
}
