using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearItem
{
    public string gearName = "Apprentice Hat";
    public FishRarity rarity = FishRarity.Common;
    public GearSlot slot = GearSlot.Hat;
    public int levelRequirement = 1;
    public GearAffix mainBonus = new GearAffix { affixType = GearAffixType.Focus, value = 1 };
    public List<GearAffix> affixes = new List<GearAffix>();
    public int sellValue = 10;
    [TextArea] public string flavorText = "A humble bit of wizard fishing fashion.";

    public string GetDisplayName()
    {
        return rarity + " " + gearName + " (" + slot + ")";
    }

    public string GetSummary()
    {
        string summary = GetDisplayName() + " | " + mainBonus.GetDescription();
        foreach (GearAffix affix in affixes)
        {
            summary += ", " + affix.GetDescription();
        }

        return summary;
    }
}
