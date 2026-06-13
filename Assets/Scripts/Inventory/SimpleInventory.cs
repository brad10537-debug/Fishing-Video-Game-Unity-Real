using System.Collections.Generic;
using UnityEngine;

// Stores the player's permanent items and fish.
// Selling is requested by economy scripts, but inventory owns removing fish records.
public class SimpleInventory : MonoBehaviour
{
    private readonly List<string> items = new List<string>();
    private readonly List<FishData> caughtFish = new List<FishData>();
    private readonly List<bool> caughtFishCanSell = new List<bool>();
    public event System.Action InventoryChanged;

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);
        InventoryChanged?.Invoke();
        PrintInventory();
    }

    public void AddFish(FishData fish)
    {
        AddFish(fish, true);
    }

    public void AddFish(FishData fish, bool canSell)
    {
        caughtFish.Add(fish);
        caughtFishCanSell.Add(canSell);
        Debug.Log("Added fish to inventory: " + fish.GetDisplayName() + (canSell ? " (sellable)" : " (collected)"));
        InventoryChanged?.Invoke();
        PrintInventory();
    }

    public void AddFishRange(IEnumerable<FishData> fishToAdd)
    {
        foreach (FishData fish in fishToAdd)
        {
            caughtFish.Add(fish);
            caughtFishCanSell.Add(true);
        }

        InventoryChanged?.Invoke();
        PrintInventory();
    }

    public void PrintInventory()
    {
        if (items.Count == 0 && caughtFish.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        Debug.Log(GetInventorySummary());
    }

    public IReadOnlyList<string> GetItems()
    {
        return items;
    }

    public IReadOnlyList<FishData> GetCaughtFish()
    {
        return caughtFish;
    }

    public int GetFishCount()
    {
        return caughtFish.Count;
    }

    public bool HasCaughtFishName(string fishName)
    {
        foreach (FishData fish in caughtFish)
        {
            if (fish.fishName == fishName)
            {
                return true;
            }
        }

        return false;
    }

    public int GetSellableFishCount()
    {
        int count = 0;
        for (int i = 0; i < caughtFishCanSell.Count; i++)
        {
            if (caughtFishCanSell[i])
            {
                count++;
            }
        }

        return count;
    }

    public int RemoveAllFishAndGetSellValue()
    {
        int totalSellValue = 0;

        for (int i = 0; i < caughtFish.Count; i++)
        {
            if (i < caughtFishCanSell.Count && caughtFishCanSell[i])
            {
                totalSellValue += caughtFish[i].sellValue;
            }
        }

        List<FishData> collectedFish = new List<FishData>();
        List<bool> collectedFlags = new List<bool>();
        for (int i = 0; i < caughtFish.Count; i++)
        {
            bool canSell = i < caughtFishCanSell.Count && caughtFishCanSell[i];
            if (!canSell)
            {
                collectedFish.Add(caughtFish[i]);
                collectedFlags.Add(false);
            }
        }

        caughtFish.Clear();
        caughtFishCanSell.Clear();
        caughtFish.AddRange(collectedFish);
        caughtFishCanSell.AddRange(collectedFlags);
        InventoryChanged?.Invoke();
        PrintInventory();
        return totalSellValue;
    }

    public string GetInventorySummary()
    {
        List<string> entries = new List<string>();

        foreach (string item in items)
        {
            entries.Add(item);
        }

        for (int i = 0; i < caughtFish.Count; i++)
        {
            FishData fish = caughtFish[i];
            bool canSell = i < caughtFishCanSell.Count && caughtFishCanSell[i];
            string valueText = canSell ? fish.sellValue + " coins" : "collected";
            entries.Add(fish.GetDisplayName() + " (" + valueText + ")");
        }

        if (entries.Count == 0)
        {
            return "Inventory: Empty";
        }

        return "Inventory: " + string.Join(", ", entries);
    }
}
