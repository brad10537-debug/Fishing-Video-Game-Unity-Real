using UnityEngine;

// Calculates one readable difficulty number for fish minigames.
// Multiplayer can later send this same calculated value to clients.
public class FishingDifficultyManager : MonoBehaviour
{
    public static FishingDifficultyManager Instance { get; private set; }

    [Header("Difficulty Scaling")]
    public int zoneLevel = 1;
    public int bossLevel = 1;
    public float roundDifficultyBonus = 0.15f;
    public float zoneDifficultyBonus = 0.2f;
    public float bossDifficultyBonus = 0.25f;
    public float playerLevelReduction = 0.04f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public float CalculateFishDifficulty(FishSpeciesData species, PlayerInteractor interactor, bool isBossFish)
    {
        float difficulty = species != null ? species.baseDifficulty : 1f;
        difficulty += GetRarityDifficulty(species != null ? species.rarity : FishRarity.Common);

        if (RunManager.Instance != null && RunManager.Instance.IsFishingRoundActive())
        {
            difficulty += Mathf.Max(0, RunManager.Instance.CurrentRound - 1) * roundDifficultyBonus;
        }

        difficulty += Mathf.Max(0, zoneLevel - 1) * zoneDifficultyBonus;

        if (isBossFish)
        {
            difficulty += Mathf.Max(0, bossLevel - 1) * bossDifficultyBonus;
        }

        if (interactor != null && interactor.progression != null)
        {
            difficulty -= Mathf.Max(0, interactor.progression.level - 1) * playerLevelReduction;
            difficulty -= Mathf.Max(0, interactor.progression.fishingSkill) * 0.03f;
        }

        if (interactor != null && interactor.fishingRod != null)
        {
            difficulty -= interactor.fishingRod.GetMinigameEaseBonus();
        }

        return Mathf.Clamp(difficulty, 0.35f, 8f);
    }

    private float GetRarityDifficulty(FishRarity rarity)
    {
        switch (rarity)
        {
            case FishRarity.Uncommon:
                return 0.05f;
            case FishRarity.Rare:
                return 0.55f;
            case FishRarity.Epic:
                return 0.95f;
            case FishRarity.Legendary:
                return 1.4f;
            case FishRarity.Mythic:
                return 2f;
            default:
                return -0.1f;
        }
    }
}
