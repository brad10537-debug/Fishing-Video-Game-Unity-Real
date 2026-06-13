using UnityEngine;

[System.Serializable]
public class LootReward
{
    public string itemName = "Gold Coin";
    [Min(0.01f)] public float weight = 1f;
}

public class LootableObjectInteractable : MonoBehaviour, IInteractable
{
    [Header("Lootable Object")]
    public string objectDisplayName = "Chest";
    public bool lootOnce = true;

    [Header("Loot")]
    public LootReward[] possibleLoot =
    {
        new LootReward { itemName = "Gold Coin", weight = 3f },
        new LootReward { itemName = "Apple", weight = 2f },
        new LootReward { itemName = "Old Key", weight = 0.5f }
    };

    [Header("Messages")]
    public string lootedStatusMessage = "You found loot.";
    public string alreadyLootedMessage = "This has already been looted.";

    private bool hasBeenLooted;

    public string GetInteractPrompt()
    {
        if (lootOnce && hasBeenLooted)
        {
            return objectDisplayName + " is empty.";
        }

        return "Press E to loot " + objectDisplayName + ".";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (lootOnce && hasBeenLooted)
        {
            ShowStatus(interactor, alreadyLootedMessage);
            return;
        }

        string reward = ChooseLoot();

        if (interactor != null && interactor.inventory != null)
        {
            interactor.inventory.AddItem(reward);
        }
        else
        {
            Debug.Log("Found " + reward + ", but no inventory was assigned.");
        }

        hasBeenLooted = true;
        ShowStatus(interactor, lootedStatusMessage + " Found: " + reward);
    }

    private string ChooseLoot()
    {
        if (possibleLoot == null || possibleLoot.Length == 0)
        {
            return "Nothing";
        }

        float totalWeight = 0f;
        foreach (LootReward loot in possibleLoot)
        {
            totalWeight += Mathf.Max(0.01f, loot.weight);
        }

        float roll = Random.Range(0f, totalWeight);
        foreach (LootReward loot in possibleLoot)
        {
            roll -= Mathf.Max(0.01f, loot.weight);
            if (roll <= 0f)
            {
                return loot.itemName;
            }
        }

        return possibleLoot[possibleLoot.Length - 1].itemName;
    }

    private void ShowStatus(PlayerInteractor interactor, string message)
    {
        if (interactor != null)
        {
            interactor.ShowStatus(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
