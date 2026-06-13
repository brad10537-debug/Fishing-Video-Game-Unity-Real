using System.Collections.Generic;
using UnityEngine;

// Prototype gear/equipment store. It owns equipped gear bonuses but does not
// replace SimpleInventory yet; future UI can call RollAndEquipRandomGear.
public class WizardGear : MonoBehaviour
{
    public List<GearItem> equippedGear = new List<GearItem>();
    public event System.Action GearChanged;

    private readonly string[] gearNames =
    {
        "Moon-Tipped Hat",
        "Frog Velvet Robe",
        "Comet Boots",
        "Rune Gloves",
        "Bubbleglass Trinket",
        "Crooked Bonus Rod"
    };

    private void Start()
    {
        if (equippedGear.Count == 0)
        {
            RollAndEquipRandomGear();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            RollAndEquipRandomGear();
        }
    }

    public GearItem RollAndEquipRandomGear()
    {
        GearItem item = GenerateRandomGear();
        Equip(item);
        Debug.Log("Rolled and equipped gear: " + item.GetSummary());
        return item;
    }

    public void Equip(GearItem item)
    {
        for (int i = equippedGear.Count - 1; i >= 0; i--)
        {
            if (equippedGear[i].slot == item.slot)
            {
                equippedGear.RemoveAt(i);
            }
        }

        equippedGear.Add(item);
        GearChanged?.Invoke();
    }

    public int GetStatBonus(WizardStatType statType)
    {
        return Mathf.RoundToInt(GetAffixValue((GearAffixType)(int)statType));
    }

    public float GetAffixValue(GearAffixType affixType)
    {
        float total = 0f;

        foreach (GearItem item in equippedGear)
        {
            if (item.mainBonus != null && item.mainBonus.affixType == affixType)
            {
                total += item.mainBonus.value;
            }

            foreach (GearAffix affix in item.affixes)
            {
                if (affix.affixType == affixType)
                {
                    total += affix.value;
                }
            }
        }

        return total;
    }

    public string GetGearSummary()
    {
        if (equippedGear.Count == 0)
        {
            return "Gear: None";
        }

        List<string> summaries = new List<string>();
        foreach (GearItem item in equippedGear)
        {
            summaries.Add(item.GetSummary());
        }

        return "Gear:\n" + string.Join("\n", summaries);
    }

    private GearItem GenerateRandomGear()
    {
        FishRarity rarity = RollGearRarity();
        GearSlot slot = (GearSlot)Random.Range(0, System.Enum.GetValues(typeof(GearSlot)).Length);
        int rarityRank = (int)rarity;
        int affixCount = rarityRank >= (int)FishRarity.Epic ? 3 : rarityRank >= (int)FishRarity.Rare ? 2 : 1;

        GearItem item = new GearItem
        {
            gearName = gearNames[Random.Range(0, gearNames.Length)],
            rarity = rarity,
            slot = slot,
            levelRequirement = 1,
            mainBonus = RollAffix(true),
            sellValue = 10 + (int)rarity * 25,
            flavorText = "Freshly rolled prototype gear with suspicious sparkle."
        };

        for (int i = 0; i < affixCount; i++)
        {
            item.affixes.Add(RollAffix(false));
        }

        return item;
    }

    private GearAffix RollAffix(bool mainBonus)
    {
        GearAffixType[] options =
        {
            GearAffixType.Focus,
            GearAffixType.Reflex,
            GearAffixType.Luck,
            GearAffixType.Control,
            GearAffixType.Power,
            GearAffixType.Wisdom,
            GearAffixType.PerfectCatchWindowPercent,
            GearAffixType.RareFishChancePercent,
            GearAffixType.BossTensionControlPercent,
            GearAffixType.SellValuePercent,
            GearAffixType.FishXPPercent,
            GearAffixType.RapidTapPowerPercent,
            GearAffixType.SequenceMistakeAllowance
        };

        GearAffixType type = options[Random.Range(0, options.Length)];
        float value = mainBonus ? Random.Range(2, 6) : Random.Range(1, 5);

        if (type.ToString().Contains("Percent"))
        {
            value *= 3f;
        }

        if (type == GearAffixType.SequenceMistakeAllowance)
        {
            value = 1f;
        }

        return new GearAffix { affixType = type, value = value };
    }

    private FishRarity RollGearRarity()
    {
        float roll = Random.value;
        if (roll < 0.50f) return FishRarity.Common;
        if (roll < 0.75f) return FishRarity.Uncommon;
        if (roll < 0.90f) return FishRarity.Rare;
        if (roll < 0.97f) return FishRarity.Epic;
        if (roll < 0.995f) return FishRarity.Legendary;
        return FishRarity.Mythic;
    }
}
