using UnityEngine;

// Stores the player's permanent coin balance.
// The field is still named gold so older Inspector references and scripts keep working.
public class PlayerCurrency : MonoBehaviour
{
    public int gold = 0;
    public event System.Action<int> GoldChanged;

    public int Coins
    {
        get { return gold; }
    }

    private void Start()
    {
        GoldChanged?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        AddCoins(amount);
    }

    public void AddCoins(int amount)
    {
        int safeAmount = Mathf.Max(0, amount);
        gold += safeAmount;
        if (safeAmount > 0 && GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayCoinGain();
        }

        Debug.Log("Coins: " + gold);
        GoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        return SpendCoins(amount);
    }

    public bool SpendCoins(int amount)
    {
        if (amount < 0 || gold < amount)
        {
            return false;
        }

        gold -= amount;
        Debug.Log("Coins: " + gold);
        GoldChanged?.Invoke(gold);
        return true;
    }
}
