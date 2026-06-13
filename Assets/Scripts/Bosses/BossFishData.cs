using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossFishData
{
    public string bossName = "Lord Bubblebeard";
    public int bossLevel = 1;
    public FishRarity bossRarity = FishRarity.Legendary;
    [TextArea] public string flavorText = "A giant magical fish with theatrical timing.";
    [TextArea] public string specialCatchMessage = "The boss fish splashes into your legend.";
    public int goldReward = 500;
    public int xpReward = 120;
    [Range(0f, 1f)] public float gearDropChance = 0.25f;

    public List<BossPhaseData> phases = new List<BossPhaseData>
    {
        new BossPhaseData { phaseName = "Phase 1", encounterType = FishEncounterType.RapidTap, difficulty = 1f, duration = 8f, captureRequired = 30f, tensionIncreasePerSecond = 4f, instructions = "Tap Space to build capture." },
        new BossPhaseData { phaseName = "Phase 2", encounterType = FishEncounterType.TimingWindow, difficulty = 1.25f, duration = 8f, captureRequired = 30f, tensionIncreasePerSecond = 5f, instructions = "Press Space inside the safe timing window." },
        new BossPhaseData { phaseName = "Final Catch", encounterType = FishEncounterType.PrecisionStop, difficulty = 1.5f, duration = 7f, captureRequired = 40f, tensionIncreasePerSecond = 6f, instructions = "Stop near the center to finish the catch." }
    };
}
