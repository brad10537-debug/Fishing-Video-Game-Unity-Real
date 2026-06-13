using UnityEngine;

[System.Serializable]
public class FishJournalEntry
{
    public string fishName = "Pond Minnow";
    public FishRarity rarity = FishRarity.Common;
    public FishBehaviorType behaviorType = FishBehaviorType.Timing;
    [TextArea] public string description = "A mysterious pond fish.";
    public int caughtCount;
    public float bestWeight;
    public int coinValue;
    public int xpValue;
    public bool discovered;

    public void RecordCatch(FishData fish)
    {
        discovered = true;
        caughtCount++;
        bestWeight = Mathf.Max(bestWeight, fish.weight);
        rarity = fish.rarity;
        behaviorType = fish.behaviorType;
        coinValue = fish.sellValue;
        xpValue = fish.xpReward;

        if (string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(fish.flavorText))
        {
            description = fish.flavorText;
        }
    }
}
