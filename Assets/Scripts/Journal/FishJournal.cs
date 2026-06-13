using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Tracks fish collection progress during Play Mode.
// This is intentionally simple and does not save permanently yet.
public class FishJournal : MonoBehaviour
{
    public List<FishJournalEntry> entries = CreateStarterEntries();

    public event System.Action JournalChanged;

    public static List<FishJournalEntry> CreateStarterEntries()
    {
        return new List<FishJournalEntry>
        {
            new FishJournalEntry { fishName = "Pond Minnow", rarity = FishRarity.Common, behaviorType = FishBehaviorType.Timing, description = "A tiny starter fish that sparkles when it sneezes.", coinValue = 8, xpValue = 6 },
            new FishJournalEntry { fishName = "Bluegill Sprite", rarity = FishRarity.Common, behaviorType = FishBehaviorType.RapidTap, description = "Half fish, half tiny pond prank.", coinValue = 14, xpValue = 8 },
            new FishJournalEntry { fishName = "Moon Carp", rarity = FishRarity.Uncommon, behaviorType = FishBehaviorType.HoldBalance, description = "Its scales reflect a moon that is not always there.", coinValue = 36, xpValue = 13 },
            new FishJournalEntry { fishName = "Crystal Trout", rarity = FishRarity.Rare, behaviorType = FishBehaviorType.ReactionSequence, description = "A glassy trout that clinks politely when caught.", coinValue = 105, xpValue = 24 },
            new FishJournalEntry { fishName = "Goblin Catfish", rarity = FishRarity.Uncommon, behaviorType = FishBehaviorType.Erratic, description = "Looks like it stole bait from its own reflection.", coinValue = 50, xpValue = 14 },
            new FishJournalEntry { fishName = "Ancient Boss Koi", rarity = FishRarity.Legendary, behaviorType = FishBehaviorType.Erratic, description = "Old enough to remember when the pond was only a puddle.", coinValue = 525, xpValue = 95 }
        };
    }

    public void ResetToStarterEntries()
    {
        entries = CreateStarterEntries();
        JournalChanged?.Invoke();
    }

    public bool RecordCatch(FishData fish)
    {
        FishJournalEntry entry = GetOrCreateEntry(fish);
        bool wasDiscovered = entry.discovered;
        entry.RecordCatch(fish);
        JournalChanged?.Invoke();
        return !wasDiscovered;
    }

    public int GetDiscoveredCount()
    {
        int count = 0;
        foreach (FishJournalEntry entry in entries)
        {
            if (entry.discovered)
            {
                count++;
            }
        }

        return count;
    }

    public string BuildJournalText()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Fish Journal");
        builder.AppendLine("Discovered: " + GetDiscoveredCount() + " / " + entries.Count);
        builder.AppendLine();

        foreach (FishJournalEntry entry in entries)
        {
            if (!entry.discovered)
            {
                builder.AppendLine("[???]");
                builder.AppendLine("Undiscovered");
                builder.AppendLine();
                continue;
            }

            builder.AppendLine("[" + entry.rarity + "] " + entry.fishName);
            builder.AppendLine("Caught: " + entry.caughtCount);
            builder.AppendLine("Best Weight: " + entry.bestWeight.ToString("0.0") + " lbs");
            builder.AppendLine("Behavior: " + GetBehaviorDisplayName(entry.behaviorType));
            builder.AppendLine("Value: " + entry.coinValue + " Coins - " + entry.xpValue + " XP");

            if (!string.IsNullOrEmpty(entry.description))
            {
                builder.AppendLine(entry.description);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private FishJournalEntry GetOrCreateEntry(FishData fish)
    {
        foreach (FishJournalEntry entry in entries)
        {
            if (entry.fishName == fish.fishName)
            {
                return entry;
            }
        }

        FishJournalEntry newEntry = new FishJournalEntry
        {
            fishName = fish.fishName,
            rarity = fish.rarity,
            behaviorType = fish.behaviorType,
            description = fish.flavorText,
            coinValue = fish.sellValue,
            xpValue = fish.xpReward
        };

        entries.Add(newEntry);
        return newEntry;
    }

    private string GetBehaviorDisplayName(FishBehaviorType behaviorType)
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
