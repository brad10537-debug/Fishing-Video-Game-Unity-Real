using UnityEngine;

// Stores player level and XP. Unlocks can listen to this later.
public class PlayerProgression : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    [Min(0)] public int fishingSkill = 0;
    public int statPointsPerLevel = 1;
    public WizardStats wizardStats;
    public PlayerCurrency currency;
    public PlayerFishingRod fishingRod;
    public event System.Action<int, int, int> ProgressionChanged;

    private void Awake()
    {
        if (wizardStats == null)
        {
            wizardStats = GetComponent<WizardStats>();
        }

        if (currency == null)
        {
            currency = GetComponent<PlayerCurrency>();
        }

        if (fishingRod == null)
        {
            fishingRod = GetComponent<PlayerFishingRod>();
        }
    }

    private void Start()
    {
        ProgressionChanged?.Invoke(level, currentXP, xpToNextLevel);
    }

    public int AddXP(int amount)
    {
        int safeAmount = Mathf.Max(0, amount);
        currentXP += safeAmount;
        if (safeAmount > 0 && GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayXpGain();
        }

        int levelsGained = 0;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            level++;
            levelsGained++;
            fishingSkill++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f);
            Debug.Log("Level up! You are now level " + level + ". Fishing Skill increased to " + fishingSkill + ".");
            if (GameAudioFeedback.Instance != null)
            {
                GameAudioFeedback.Instance.PlayLevelUp();
            }

            if (wizardStats != null)
            {
                wizardStats.AddStatPoints(statPointsPerLevel);
            }
        }

        Debug.Log("XP: " + currentXP + " / " + xpToNextLevel);
        ProgressionChanged?.Invoke(level, currentXP, xpToNextLevel);
        return levelsGained;
    }

    public int GetCoins()
    {
        return currency != null ? currency.Coins : 0;
    }

    public int GetRodLevel()
    {
        return fishingRod != null ? fishingRod.rodLevel : 1;
    }
}
