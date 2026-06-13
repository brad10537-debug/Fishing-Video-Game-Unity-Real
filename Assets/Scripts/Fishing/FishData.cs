using UnityEngine;

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

[System.Serializable]
public struct FishData
{
    public string fishName;
    public FishRarity rarity;
    public string color;
    public float weight;
    public int magicPower;
    public int sellValue;
    public int xpReward;
    public FishEncounterType encounterType;
    public FishBehaviorType behaviorType;
    public CatchQuality catchQuality;
    [TextArea] public string flavorText;
    public string specialTrait;

    public string GetDisplayName()
    {
        return rarity + " " + color + " " + fishName;
    }

    // Keeps rarity-specific catch text in one place for UI and debug logs.
    public string GetCatchMessage()
    {
        switch (rarity)
        {
            case FishRarity.Mythic:
                return "REALITY SPARKLES! You caught a " + GetDisplayName() + "!";
            case FishRarity.Legendary:
                return "Legendary catch! You caught a " + GetDisplayName() + "!";
            case FishRarity.Epic:
                return "The pond flashes with funky magic. You caught a " + GetDisplayName() + "!";
            case FishRarity.Rare:
                return "Nice catch! You caught a " + GetDisplayName() + "!";
            case FishRarity.Uncommon:
                return "A magical ripple appears. You caught a " + GetDisplayName() + "!";
            default:
                return "You caught a " + GetDisplayName() + "!";
        }
    }

    public string GetCatchDetails()
    {
        return GetCatchMessage() + "\n"
            + "Weight: " + weight.ToString("0.0") + " lbs\n"
            + "Magic Power: " + magicPower + "\n"
            + "Coin Value: " + sellValue + " coins\n"
            + "XP: " + xpReward + "\n"
            + "Quality: " + catchQuality + "\n"
            + "Behavior: " + GetBehaviorDisplayName() + "\n"
            + "Encounter: " + encounterType + "\n"
            + "Trait: " + specialTrait + "\n"
            + flavorText;
    }

    public string GetBehaviorDisplayName()
    {
        switch (behaviorType)
        {
            case FishBehaviorType.RapidTap:
                return "Rapid Tap";
            case FishBehaviorType.HoldBalance:
                return "Hold Balance";
            case FishBehaviorType.ReactionSequence:
                return "Reaction Sequence";
            case FishBehaviorType.Erratic:
                return "Erratic";
            default:
                return "Timing";
        }
    }
}
