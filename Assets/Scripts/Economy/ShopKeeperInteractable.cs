using System.Text;
using UnityEngine;

// Text-based prototype shop. It sells fish through SimpleInventory,
// spends coins through PlayerCurrency, and equips/upgrades rods through PlayerFishingRod.
public class ShopKeeperInteractable : MonoBehaviour, IInteractable
{
    public string shopkeeperName = "Mordo the Rodmancer";

    private PlayerInteractor activeInteractor;
    private bool shopOpen;

    private void Update()
    {
        if (!shopOpen || activeInteractor == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SellAllFish();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UpgradeRodLevel();
            return;
        }

        int rodCount = activeInteractor.fishingRod != null ? Mathf.Min(activeInteractor.fishingRod.rods.Count, 8) : 0;
        for (int i = 0; i < rodCount; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha2 + i);
            if (Input.GetKeyDown(key))
            {
                BuyOrEquipRod(i);
                return;
            }
        }
    }

    public string GetInteractPrompt()
    {
        if (shopOpen)
        {
            return shopkeeperName + "'s shop is open.";
        }

        return "Press E to talk to " + shopkeeperName + ".";
    }

    public void Interact(PlayerInteractor interactor)
    {
        activeInteractor = interactor;
        shopOpen = true;
        string welcome = "Welcome to " + shopkeeperName + "'s rod shop.";
        if (activeInteractor != null)
        {
            activeInteractor.ShowShopPopup(BuildShortUpgradeSummary(), true);
        }

        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayShopOpen();
        }

        ShowShopMenu(welcome);
    }

    private void SellAllFish()
    {
        if (activeInteractor.inventory == null || activeInteractor.currency == null)
        {
            ShowShopMenu("Missing inventory or currency on the player.");
            return;
        }

        int fishCount = activeInteractor.inventory.GetSellableFishCount();
        int earnedGold = activeInteractor.inventory.RemoveAllFishAndGetSellValue();

        if (fishCount == 0)
        {
            ShowShopMenu("You have no sellable fish. Starter pond catches already paid coins when caught.");
            return;
        }

        activeInteractor.currency.AddCoins(earnedGold);
        ShowShopMenu("Sold " + fishCount + " fish for " + earnedGold + " coins.");
    }

    private void BuyOrEquipRod(int rodIndex)
    {
        if (activeInteractor.fishingRod == null)
        {
            ShowShopMenu("Missing PlayerFishingRod on the player.");
            return;
        }

        activeInteractor.fishingRod.TryBuyOrEquipRod(rodIndex, activeInteractor.currency, out string message);
        ShowShopMenu(message);
    }

    private void UpgradeRodLevel()
    {
        if (activeInteractor.fishingRod == null)
        {
            ShowShopMenu("Missing PlayerFishingRod on the player.");
            return;
        }

        bool success = activeInteractor.fishingRod.TryUpgradeRodLevel(activeInteractor.currency, out string message);
        if (activeInteractor != null)
        {
            activeInteractor.ShowShopPopup(message, success);
        }

        ShowShopMenu(message);
    }

    private void CloseShop()
    {
        shopOpen = false;

        if (activeInteractor != null && activeInteractor.prototypeHud != null)
        {
            activeInteractor.prototypeHud.SetShopMenu("");
        }

        if (activeInteractor != null)
        {
            activeInteractor.ShowStatus("Shop closed.");
        }

        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayShopClose();
        }

        activeInteractor = null;
    }

    private void ShowShopMenu(string statusMessage)
    {
        if (activeInteractor == null)
        {
            Debug.Log(statusMessage);
            return;
        }

        activeInteractor.ShowStatus(statusMessage);

        if (activeInteractor.prototypeHud != null)
        {
            activeInteractor.prototypeHud.SetShopMenu(BuildShopMenu(statusMessage));
        }
        else
        {
            Debug.Log(BuildShopMenu(statusMessage));
        }
    }

    private string BuildShopMenu(string statusMessage)
    {
        StringBuilder builder = new StringBuilder();
        int coins = activeInteractor != null && activeInteractor.currency != null ? activeInteractor.currency.Coins : 0;
        int rodLevel = activeInteractor != null && activeInteractor.fishingRod != null ? activeInteractor.fishingRod.rodLevel : 1;
        int upgradeCost = activeInteractor != null && activeInteractor.fishingRod != null ? activeInteractor.fishingRod.GetRodLevelUpgradeCost() : 0;
        bool canAffordUpgrade = coins >= upgradeCost;

        builder.AppendLine(statusMessage);
        builder.AppendLine("Coins: " + coins);
        builder.AppendLine("Current Rod Level: " + rodLevel);

        if (activeInteractor != null && activeInteractor.fishingRod != null)
        {
            builder.AppendLine("Rod Upgrade Cost: " + upgradeCost + " coins");
            builder.AppendLine("Upgrade Effect: makes fishing minigames slightly easier.");
            builder.AppendLine("Press 0 to upgrade rod" + (canAffordUpgrade ? "" : " (not enough coins)"));
        }

        builder.AppendLine("1 - Sell All Fish");

        PlayerFishingRod rods = activeInteractor != null ? activeInteractor.fishingRod : null;
        if (rods != null)
        {
            for (int i = 0; i < rods.rods.Count && i < 8; i++)
            {
                FishingRodData rod = rods.rods[i];
                string ownedText = rod.owned ? "Owned" : rod.cost + " coins";
                string equippedText = i == rods.equippedRodIndex ? " Equipped" : "";
                builder.AppendLine((i + 2) + " - " + rod.rodName + " [" + ownedText + "]" + equippedText);
                builder.AppendLine("    Rarity +" + rod.rarityBonus + ", Magic +" + rod.magicPowerBonus + ", Boss +" + rod.bossCatchBonus + " - " + rod.description);
            }
        }

        builder.AppendLine("Esc - Close Shop");
        return builder.ToString();
    }

    private string BuildShortUpgradeSummary()
    {
        int coins = activeInteractor != null && activeInteractor.currency != null ? activeInteractor.currency.Coins : 0;
        int rodLevel = activeInteractor != null && activeInteractor.fishingRod != null ? activeInteractor.fishingRod.rodLevel : 1;
        int upgradeCost = activeInteractor != null && activeInteractor.fishingRod != null ? activeInteractor.fishingRod.GetRodLevelUpgradeCost() : 0;

        return "Mordo's Rod Shop\n"
            + "Current Rod Level: " + rodLevel + "\n"
            + "Upgrade Cost: " + upgradeCost + " Coins\n"
            + "Your Coins: " + coins + "\n"
            + "Press 0 to upgrade rod";
    }
}
