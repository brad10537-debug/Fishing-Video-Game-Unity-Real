using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishingRodData
{
    public string rodName = "Twig Wand Rod";
    [Min(0)] public int cost = 0;
    [Min(0f)] public float rarityBonus = 0f;
    [Min(0)] public int magicPowerBonus = 0;
    [Min(0f)] public float bossCatchBonus = 0f;
    [TextArea] public string description = "A humble starter rod with a faint magical wobble.";
    public bool owned = false;
}

public class PlayerFishingRod : MonoBehaviour
{
    // Owns rod ownership/equipping and exposes bonuses used by fishing and bosses.
    [Header("Starter Upgrade Loop")]
    [Min(1)] public int rodLevel = 1;
    [Min(0)] public int baseRodLevelUpgradeCost = 50;
    [Min(0f)] public float minigameEasePerRodLevel = 0.12f;

    public List<FishingRodData> rods = new List<FishingRodData>
    {
        new FishingRodData { rodName = "Twig Wand Rod", cost = 0, rarityBonus = 0f, magicPowerBonus = 0, bossCatchBonus = 0f, description = "A crooked twig that believes in you.", owned = true },
        new FishingRodData { rodName = "Copper Moon Rod", cost = 75, rarityBonus = 2f, magicPowerBonus = 5, bossCatchBonus = 1f, description = "A warm copper rod that glows under moonlight." },
        new FishingRodData { rodName = "Crystal Tide Rod", cost = 180, rarityBonus = 5f, magicPowerBonus = 12, bossCatchBonus = 2f, description = "A clear crystal rod that hums near magical water." },
        new FishingRodData { rodName = "Neon Wizard Rod", cost = 420, rarityBonus = 10f, magicPowerBonus = 25, bossCatchBonus = 4f, description = "A loud, bright rod for stylish spell-fishers." },
        new FishingRodData { rodName = "Ancient Star Rod", cost = 900, rarityBonus = 18f, magicPowerBonus = 45, bossCatchBonus = 7f, description = "An old star-metal rod with impossible shimmer." },
        new FishingRodData { rodName = "Frog King Rod", cost = 1250, rarityBonus = 22f, magicPowerBonus = 60, bossCatchBonus = 10f, description = "A royal swamp rod that ribbits during rare catches." },
        new FishingRodData { rodName = "Turtle Sage Rod", cost = 1600, rarityBonus = 28f, magicPowerBonus = 75, bossCatchBonus = 14f, description = "A patient old rod for catching impossible pond legends." }
    };

    public int equippedRodIndex = 0;
    public event System.Action<FishingRodData> EquippedRodChanged;
    public event System.Action<int> RodLevelChanged;

    private void Awake()
    {
        EnsureValidRodSetup();
    }

    private void Start()
    {
        EquippedRodChanged?.Invoke(GetEquippedRod());
        RodLevelChanged?.Invoke(rodLevel);
    }

    public FishingRodData GetEquippedRod()
    {
        EnsureValidRodSetup();
        return rods[equippedRodIndex];
    }

    public bool TryBuyOrEquipRod(int rodIndex, PlayerCurrency currency, out string message)
    {
        EnsureValidRodSetup();

        if (rodIndex < 0 || rodIndex >= rods.Count)
        {
            message = "That rod is not for sale.";
            return false;
        }

        FishingRodData rod = rods[rodIndex];

        if (!rod.owned)
        {
            if (currency == null)
            {
                message = "You need a coin pouch before buying rods.";
                return false;
            }

            if (!currency.SpendCoins(rod.cost))
            {
                message = "Not enough coins for " + rod.rodName + ". Cost: " + rod.cost + " coins.";
                return false;
            }

            rod.owned = true;
            message = "Bought and equipped " + rod.rodName + "!";
        }
        else
        {
            message = "Equipped " + rod.rodName + "!";
        }

        equippedRodIndex = rodIndex;
        EquippedRodChanged?.Invoke(rod);
        return true;
    }

    public int GetRodLevelUpgradeCost()
    {
        return baseRodLevelUpgradeCost * Mathf.Max(1, rodLevel);
    }

    public float GetMinigameEaseBonus()
    {
        return Mathf.Max(0, rodLevel - 1) * minigameEasePerRodLevel;
    }

    public bool TryUpgradeRodLevel(PlayerCurrency currency, out string message)
    {
        if (currency == null)
        {
            message = "You need a coin pouch before upgrading your rod level.";
            return false;
        }

        int cost = GetRodLevelUpgradeCost();
        if (!currency.SpendCoins(cost))
        {
            message = "Not enough coins for Rod Level " + (rodLevel + 1) + ". Cost: " + cost + " coins.";
            return false;
        }

        rodLevel++;
        message = "Rod upgraded to Level " + rodLevel + "!\nCurrent Rod Level: " + rodLevel + "\nFuture fishing minigames are slightly easier.";
        RodLevelChanged?.Invoke(rodLevel);
        EquippedRodChanged?.Invoke(GetEquippedRod());
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayRodUpgrade();
        }

        return true;
    }

    private void EnsureValidRodSetup()
    {
        if (rods == null)
        {
            rods = new List<FishingRodData>();
        }

        if (rods.Count == 0)
        {
            rods.Add(new FishingRodData { rodName = "Twig Wand Rod", owned = true });
        }

        rods[0].owned = true;
        equippedRodIndex = Mathf.Clamp(equippedRodIndex, 0, rods.Count - 1);

        if (!rods[equippedRodIndex].owned)
        {
            equippedRodIndex = 0;
        }

        rodLevel = Mathf.Max(1, rodLevel);
    }
}
