using UnityEngine;

[System.Serializable]
public class FishRaritySettings
{
    public FishRarity rarity = FishRarity.Common;
    [Min(0.01f)] public float rollWeight = 50f;
    public string rarityColor = "soft white/gray";
    public string[] colorAdjectives = { "Soft", "Misty", "Pebble" };
    public Vector2 weightRange = new Vector2(1f, 5f);
    public Vector2Int magicPowerRange = new Vector2Int(1, 15);
    public Vector2Int sellValueRange = new Vector2Int(5, 20);
    [TextArea] public string catchMessageStyle = "A humble magical catch.";
}

public class FishingSpotInteractable : MonoBehaviour, IInteractable
{
    // Generates fish rewards. During a run, catches are reported to RunManager;
    // outside a run, catches go directly into SimpleInventory.
    [Header("Fishing")]
    public string spotName = "Funky Wizard Pond";
    [Min(0.1f)] public float fishingTime = 2f;
    public bool awardCoinsImmediately = true;

    [Header("Optional ScriptableObject Fish")]
    public FishSpeciesAsset[] fishSpeciesAssets;

    [Header("Starter Fish Pool")]
    public FishSpeciesData[] speciesPool = CreateStarterSpeciesPool();

    [Header("Visual Feedback")]
    public Transform feedbackVisual;
    public float focusedScale = 1.15f;
    public float fishingStartPulseScale = 1.35f;
    public float pulseSeconds = 0.28f;

    public static FishSpeciesData[] CreateStarterSpeciesPool()
    {
        return new FishSpeciesData[]
        {
        new FishSpeciesData { speciesName = "Pond Minnow", rarity = FishRarity.Common, behaviorType = FishBehaviorType.Timing, encounterType = FishEncounterType.TimingWindow, baseDifficulty = 0.45f, speedModifier = 0.75f, requiredSuccessScore = 1, failureTolerance = 3, preferredHabitat = "Wizard Village Pond", flavorText = "A tiny starter fish that sparkles when it sneezes.", weightRange = new Vector2(0.5f, 2f), magicPowerRange = new Vector2Int(1, 8), sellValueRange = new Vector2Int(6, 12), xpRewardRange = new Vector2Int(4, 8) },
        new FishSpeciesData { speciesName = "Bluegill Sprite", rarity = FishRarity.Common, behaviorType = FishBehaviorType.RapidTap, encounterType = FishEncounterType.RapidTap, baseDifficulty = 0.6f, speedModifier = 0.9f, requiredSuccessScore = 5, failureTolerance = 3, preferredHabitat = "Wizard Village Pond", flavorText = "Half fish, half tiny pond prank.", weightRange = new Vector2(1f, 3.5f), magicPowerRange = new Vector2Int(3, 12), sellValueRange = new Vector2Int(10, 20), xpRewardRange = new Vector2Int(6, 10) },
        new FishSpeciesData { speciesName = "Moon Carp", rarity = FishRarity.Uncommon, behaviorType = FishBehaviorType.HoldBalance, encounterType = FishEncounterType.HoldBalance, baseDifficulty = 0.95f, speedModifier = 0.95f, requiredSuccessScore = 3, failureTolerance = 2, preferredHabitat = "Moonlit Pond", flavorText = "Its scales reflect a moon that is not always there.", weightRange = new Vector2(3f, 8f), magicPowerRange = new Vector2Int(10, 28), sellValueRange = new Vector2Int(24, 50), xpRewardRange = new Vector2Int(10, 16) },
        new FishSpeciesData { speciesName = "Crystal Trout", rarity = FishRarity.Rare, behaviorType = FishBehaviorType.ReactionSequence, encounterType = FishEncounterType.SequenceInput, baseDifficulty = 1.25f, speedModifier = 1.1f, requiredSuccessScore = 3, failureTolerance = 1, preferredHabitat = "Crystal Water", flavorText = "It makes a polite glass clink when caught.", weightRange = new Vector2(5f, 12f), magicPowerRange = new Vector2Int(25, 55), sellValueRange = new Vector2Int(70, 140), xpRewardRange = new Vector2Int(18, 30) },
        new FishSpeciesData { speciesName = "Goblin Catfish", rarity = FishRarity.Uncommon, behaviorType = FishBehaviorType.Erratic, encounterType = FishEncounterType.ReactionDodge, baseDifficulty = 1.35f, speedModifier = 1.25f, requiredSuccessScore = 3, failureTolerance = 1, preferredHabitat = "Muddy River Edge", flavorText = "It looks like it stole bait from its own reflection.", weightRange = new Vector2(4f, 10f), magicPowerRange = new Vector2Int(12, 35), sellValueRange = new Vector2Int(28, 75), xpRewardRange = new Vector2Int(10, 18) },
        new FishSpeciesData { speciesName = "Ancient Boss Koi", rarity = FishRarity.Legendary, behaviorType = FishBehaviorType.Erratic, encounterType = FishEncounterType.HoldBalance, baseDifficulty = 2.4f, speedModifier = 1.25f, requiredSuccessScore = 5, failureTolerance = 1, preferredHabitat = "Moonlit Boss Swamp", flavorText = "Old enough to remember when the pond was only a puddle. This is a future boss-style preview fish.", weightRange = new Vector2(18f, 35f), magicPowerRange = new Vector2Int(85, 140), sellValueRange = new Vector2Int(350, 700), xpRewardRange = new Vector2Int(70, 120) },
        new FishSpeciesData { speciesName = "Neon Starfish", rarity = FishRarity.Epic, behaviorType = FishBehaviorType.Erratic, encounterType = FishEncounterType.ReactionDodge, baseDifficulty = 1.8f, speedModifier = 1.3f, requiredSuccessScore = 4, failureTolerance = 1, preferredHabitat = "Neon Reef", flavorText = "It flashes like a sign outside a wizard diner.", weightRange = new Vector2(4f, 11f), magicPowerRange = new Vector2Int(55, 88), sellValueRange = new Vector2Int(150, 300), xpRewardRange = new Vector2Int(30, 50) },
        new FishSpeciesData { speciesName = "Frogscale Bass", rarity = FishRarity.Uncommon, behaviorType = FishBehaviorType.RapidTap, encounterType = FishEncounterType.RapidTap, baseDifficulty = 1f, speedModifier = 1f, requiredSuccessScore = 12, failureTolerance = 3, preferredHabitat = "Swamp Pond", flavorText = "It croaks whenever it disagrees with your rod technique.", weightRange = new Vector2(3f, 9f), magicPowerRange = new Vector2Int(12, 35), sellValueRange = new Vector2Int(25, 70), xpRewardRange = new Vector2Int(8, 16) },
        new FishSpeciesData { speciesName = "Turtleback Trout", rarity = FishRarity.Rare, behaviorType = FishBehaviorType.HoldBalance, encounterType = FishEncounterType.HoldBalance, baseDifficulty = 1.3f, speedModifier = 0.9f, requiredSuccessScore = 4, failureTolerance = 2, preferredHabitat = "Old Pond", flavorText = "A slow fish with an ancient little shell ridge.", weightRange = new Vector2(8f, 18f), magicPowerRange = new Vector2Int(28, 60), sellValueRange = new Vector2Int(75, 160), xpRewardRange = new Vector2Int(16, 30) },
        new FishSpeciesData { speciesName = "Ember Koi", rarity = FishRarity.Legendary, behaviorType = FishBehaviorType.Timing, encounterType = FishEncounterType.PrecisionStop, baseDifficulty = 2.2f, speedModifier = 1.4f, requiredSuccessScore = 2, failureTolerance = 1, preferredHabitat = "Warm Spring", flavorText = "Warm enough to toast marshmallows, not that it approves.", weightRange = new Vector2(15f, 30f), magicPowerRange = new Vector2Int(85, 130), sellValueRange = new Vector2Int(320, 680), xpRewardRange = new Vector2Int(60, 90) },
        new FishSpeciesData { speciesName = "Ghost Guppy", rarity = FishRarity.Rare, behaviorType = FishBehaviorType.Erratic, encounterType = FishEncounterType.ReactionDodge, baseDifficulty = 1.5f, speedModifier = 1.5f, requiredSuccessScore = 3, failureTolerance = 1, preferredHabitat = "Haunted Pond", flavorText = "Mostly transparent, fully dramatic.", weightRange = new Vector2(1f, 5f), magicPowerRange = new Vector2Int(35, 65), sellValueRange = new Vector2Int(80, 155), xpRewardRange = new Vector2Int(18, 32) },
        new FishSpeciesData { speciesName = "Astral Catfish", rarity = FishRarity.Epic, behaviorType = FishBehaviorType.HoldBalance, encounterType = FishEncounterType.HoldBalance, baseDifficulty = 1.9f, speedModifier = 1.15f, requiredSuccessScore = 5, failureTolerance = 1, preferredHabitat = "Star Pond", flavorText = "Its whiskers point toward suspicious constellations.", weightRange = new Vector2(12f, 24f), magicPowerRange = new Vector2Int(60, 95), sellValueRange = new Vector2Int(170, 330), xpRewardRange = new Vector2Int(35, 55) },
        new FishSpeciesData { speciesName = "Sunfire Leviathan", rarity = FishRarity.Legendary, behaviorType = FishBehaviorType.RapidTap, encounterType = FishEncounterType.RapidTap, baseDifficulty = 2.4f, speedModifier = 1.35f, requiredSuccessScore = 22, failureTolerance = 1, preferredHabitat = "Solar Basin", flavorText = "A huge blazing fish that makes shadows sweat.", weightRange = new Vector2(25f, 45f), magicPowerRange = new Vector2Int(95, 145), sellValueRange = new Vector2Int(380, 750), xpRewardRange = new Vector2Int(70, 105) },
        new FishSpeciesData { speciesName = "Rainbow Rune Eel", rarity = FishRarity.Mythic, behaviorType = FishBehaviorType.ReactionSequence, encounterType = FishEncounterType.SequenceInput, baseDifficulty = 2.8f, speedModifier = 1.5f, requiredSuccessScore = 5, failureTolerance = 0, preferredHabitat = "Rune Current", flavorText = "Every wiggle writes a spell nobody remembers.", weightRange = new Vector2(20f, 55f), magicPowerRange = new Vector2Int(130, 210), sellValueRange = new Vector2Int(700, 1500), xpRewardRange = new Vector2Int(120, 180) },
        new FishSpeciesData { speciesName = "Ancient Pondwyrm", rarity = FishRarity.Mythic, behaviorType = FishBehaviorType.ReactionSequence, encounterType = FishEncounterType.SequenceInput, baseDifficulty = 3f, speedModifier = 1.2f, requiredSuccessScore = 6, failureTolerance = 0, preferredHabitat = "Deep Old Water", flavorText = "Technically a fish, spiritually a weather event.", weightRange = new Vector2(35f, 70f), magicPowerRange = new Vector2Int(150, 240), sellValueRange = new Vector2Int(900, 1800), xpRewardRange = new Vector2Int(140, 220) }
        };
    }

    [Header("Flavor Text")]
    [TextArea] public string[] flavorTexts =
    {
        "It hums like a tiny enchanted lute.",
        "Its scales shimmer with suspicious confidence.",
        "A fish beloved by eccentric river wizards.",
        "It smells faintly of peppermint and thunder.",
        "It blinks like it knows your future outfit."
    };

    [Header("Special Traits")]
    public string[] specialTraits =
    {
        "Glittering Scales",
        "Potion-Scented",
        "Moon-Touched",
        "Sings in Bubbles",
        "Hat-Shaped Aura",
        "Tiny Thundercloud",
        "Wiggles Backwards"
    };

    [Header("Rarity Settings")]
    public FishRaritySettings[] raritySettings =
    {
        new FishRaritySettings { rarity = FishRarity.Common, rollWeight = 55f, rarityColor = "soft white/gray", colorAdjectives = new[] { "Soft", "Foggy", "Pebble", "Mushroom" }, weightRange = new Vector2(1f, 5f), magicPowerRange = new Vector2Int(1, 15), sellValueRange = new Vector2Int(5, 25), catchMessageStyle = "A sweet little pond oddity." },
        new FishRaritySettings { rarity = FishRarity.Uncommon, rollWeight = 25f, rarityColor = "green", colorAdjectives = new[] { "Mossy", "Emerald", "Fern", "Lime" }, weightRange = new Vector2(3f, 9f), magicPowerRange = new Vector2Int(12, 35), sellValueRange = new Vector2Int(20, 70), catchMessageStyle = "The water burps a green sparkle." },
        new FishRaritySettings { rarity = FishRarity.Rare, rollWeight = 12f, rarityColor = "blue/cyan", colorAdjectives = new[] { "Azure", "Cyan", "Moonlit", "Crystal" }, weightRange = new Vector2(6f, 14f), magicPowerRange = new Vector2Int(30, 60), sellValueRange = new Vector2Int(60, 150), catchMessageStyle = "A bright ripple rings across the pond." },
        new FishRaritySettings { rarity = FishRarity.Epic, rollWeight = 5f, rarityColor = "purple/magenta", colorAdjectives = new[] { "Neon", "Magenta", "Velvet", "Prismatic" }, weightRange = new Vector2(10f, 22f), magicPowerRange = new Vector2Int(55, 90), sellValueRange = new Vector2Int(140, 320), catchMessageStyle = "The pond briefly becomes a dance floor." },
        new FishRaritySettings { rarity = FishRarity.Legendary, rollWeight = 2f, rarityColor = "orange/gold", colorAdjectives = new[] { "Golden", "Sunfire", "Ember", "Royal" }, weightRange = new Vector2(18f, 35f), magicPowerRange = new Vector2Int(85, 130), sellValueRange = new Vector2Int(300, 700), catchMessageStyle = "Village bells ring from nowhere." },
        new FishRaritySettings { rarity = FishRarity.Mythic, rollWeight = 1f, rarityColor = "rainbow/neon/glowing", colorAdjectives = new[] { "Rainbow", "Cosmic", "Glowing", "Runic Neon" }, weightRange = new Vector2(30f, 60f), magicPowerRange = new Vector2Int(120, 200), sellValueRange = new Vector2Int(650, 1500), catchMessageStyle = "Wizard reality bends around the line." }
    };

    private bool isFishing;
    private Vector3 feedbackBaseScale = Vector3.one;
    private bool isFocused;
    private Coroutine feedbackPulseRoutine;

    private void Awake()
    {
        CacheFeedbackVisual();
    }

    private void OnValidate()
    {
        if (raritySettings == null)
        {
            return;
        }

        foreach (FishRaritySettings settings in raritySettings)
        {
            settings.weightRange = SortRange(settings.weightRange);
            settings.magicPowerRange = SortRange(settings.magicPowerRange);
            settings.sellValueRange = SortRange(settings.sellValueRange);
        }
    }

    public string GetInteractPrompt()
    {
        if (isFishing)
        {
            return "Fishing at " + spotName + "...";
        }

        return "Press E to fish at " + spotName + ".";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (isFishing)
        {
            ShowStatus(interactor, "Already fishing. Wait for the result.");
            return;
        }

        PulseFishingStart();
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayFishingCast();
        }

        StartCoroutine(Fish(interactor));
    }

    public void SetFocused(bool focused)
    {
        CacheFeedbackVisual();
        isFocused = focused;

        if (feedbackVisual == null || isFishing)
        {
            return;
        }

        feedbackVisual.localScale = feedbackBaseScale * (isFocused ? focusedScale : 1f);
    }

    private System.Collections.IEnumerator Fish(PlayerInteractor interactor)
    {
        isFishing = true;
        if (interactor != null && interactor.movementController != null)
        {
            interactor.movementController.SetMovementState(PlayerMovementState.Fishing);
        }

        ShowStatus(interactor, "You cast your line into " + spotName + "...");

        yield return new WaitForSeconds(fishingTime);

        FishingRodData rod = interactor != null && interactor.fishingRod != null
            ? interactor.fishingRod.GetEquippedRod()
            : null;
        float runRarityBonus = RunManager.Instance != null ? RunManager.Instance.GetRunRarityBonus() : 0f;
        runRarityBonus += GetLuckRarityBonus(interactor);
        FishSpeciesData species = ChooseSpecies(rod, runRarityBonus);
        FishData fish = GenerateFish(species, rod, interactor);
        float difficulty = FishingDifficultyManager.Instance != null
            ? FishingDifficultyManager.Instance.CalculateFishDifficulty(species, interactor, false)
            : species.baseDifficulty;

        if (FishingMinigameManager.Instance != null)
        {
            FishingMinigameManager.Instance.StartMinigame(fish, species, difficulty, interactor, (resultFish, quality) =>
            {
                CompleteFishingResult(interactor, resultFish, quality);
            });
        }
        else
        {
            CompleteFishingResult(interactor, fish, CatchQuality.Good);
        }
    }

    private void CacheFeedbackVisual()
    {
        if (feedbackVisual == null)
        {
            Transform visual = transform.Find("Visual Marker");
            feedbackVisual = visual != null ? visual : transform;
        }

        if (feedbackVisual != null && feedbackBaseScale == Vector3.one)
        {
            feedbackBaseScale = feedbackVisual.localScale;
        }
    }

    private void PulseFishingStart()
    {
        CacheFeedbackVisual();
        if (feedbackVisual == null)
        {
            return;
        }

        if (feedbackPulseRoutine != null)
        {
            StopCoroutine(feedbackPulseRoutine);
        }

        feedbackPulseRoutine = StartCoroutine(PulseFeedbackVisual());
    }

    private System.Collections.IEnumerator PulseFeedbackVisual()
    {
        float timer = 0f;
        Vector3 startScale = feedbackBaseScale * fishingStartPulseScale;
        Vector3 endScale = feedbackBaseScale;

        while (timer < pulseSeconds)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / Mathf.Max(0.01f, pulseSeconds));
            feedbackVisual.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        feedbackVisual.localScale = feedbackBaseScale * (isFocused ? focusedScale : 1f);
        feedbackPulseRoutine = null;
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

    private FishData GenerateFish(FishSpeciesData species, FishingRodData rod, PlayerInteractor interactor)
    {
        FishRaritySettings settings = GetRaritySettings(species.rarity);
        string flavorText = !string.IsNullOrEmpty(species.flavorText) ? species.flavorText : ChooseRandomText(flavorTexts, "A strange catch from strange waters.");
        string specialTrait = Random.value < GetSpecialTraitChance(interactor) ? ChooseRandomText(specialTraits, "No Special Trait") : "No Special Trait";
        int magicPowerBonus = rod != null ? rod.magicPowerBonus : 0;

        return new FishData
        {
            fishName = species.speciesName,
            rarity = species.rarity,
            color = ChooseRandomText(settings.colorAdjectives, settings.rarityColor),
            weight = Random.Range(species.weightRange.x, species.weightRange.y),
            magicPower = Random.Range(species.magicPowerRange.x, species.magicPowerRange.y + 1) + magicPowerBonus,
            sellValue = Random.Range(species.sellValueRange.x, species.sellValueRange.y + 1),
            xpReward = Random.Range(species.xpRewardRange.x, species.xpRewardRange.y + 1),
            encounterType = species.encounterType,
            behaviorType = species.behaviorType,
            catchQuality = CatchQuality.Good,
            flavorText = settings.catchMessageStyle + " " + flavorText,
            specialTrait = specialTrait
        };
    }

    private FishSpeciesData ChooseSpecies(FishingRodData rod, float runRarityBonus)
    {
        FishRaritySettings rarity = ChooseRaritySettings(rod, runRarityBonus);

        FishSpeciesData[] availableSpecies = GetAvailableSpecies();

        if (availableSpecies == null || availableSpecies.Length == 0)
        {
            return new FishSpeciesData { rarity = rarity.rarity };
        }

        int matchingCount = 0;
        foreach (FishSpeciesData species in availableSpecies)
        {
            if (species.rarity == rarity.rarity)
            {
                matchingCount++;
            }
        }

        if (matchingCount == 0)
        {
            return availableSpecies[Random.Range(0, availableSpecies.Length)];
        }

        int target = Random.Range(0, matchingCount);
        foreach (FishSpeciesData species in availableSpecies)
        {
            if (species.rarity != rarity.rarity)
            {
                continue;
            }

            if (target == 0)
            {
                return species;
            }

            target--;
        }

        return availableSpecies[0];
    }

    private FishSpeciesData[] GetAvailableSpecies()
    {
        if (fishSpeciesAssets != null && fishSpeciesAssets.Length > 0)
        {
            System.Collections.Generic.List<FishSpeciesData> assetSpecies = new System.Collections.Generic.List<FishSpeciesData>();
            foreach (FishSpeciesAsset asset in fishSpeciesAssets)
            {
                if (asset != null)
                {
                    assetSpecies.Add(asset.ToRuntimeData());
                }
            }

            if (assetSpecies.Count > 0)
            {
                return assetSpecies.ToArray();
            }
        }

        return speciesPool;
    }

    private FishRaritySettings GetRaritySettings(FishRarity rarity)
    {
        if (raritySettings == null || raritySettings.Length == 0)
        {
            return new FishRaritySettings { rarity = rarity };
        }

        foreach (FishRaritySettings settings in raritySettings)
        {
            if (settings.rarity == rarity)
            {
                return settings;
            }
        }

        return raritySettings[0];
    }

    private void CompleteFishingResult(PlayerInteractor interactor, FishData fish, CatchQuality quality)
    {
        isFishing = false;
        if (interactor != null && interactor.movementController != null && !interactor.movementController.IsSitting())
        {
            interactor.movementController.SetMovementState(PlayerMovementState.Idle);
        }

        fish = ApplyCatchQuality(fish, quality);
        fish = ApplyWizardRewards(interactor, fish);

        if (quality == CatchQuality.Failed)
        {
            string reason = FishingMinigameManager.Instance != null ? FishingMinigameManager.Instance.GetLastFailureReason() : "";
            if (interactor != null)
            {
                interactor.ShowFishingFailure(reason);
            }
            else
            {
                Debug.Log("The fish got away! " + reason);
            }
            return;
        }

        int coinsGained = 0;
        int xpGained = fish.xpReward;
        int levelsGained = 0;
        bool isNewCatch = interactor == null || interactor.inventory == null || !interactor.inventory.HasCaughtFishName(fish.fishName);

        if (interactor != null && interactor.fishJournal != null)
        {
            isNewCatch = interactor.fishJournal.RecordCatch(fish);
        }

        if (RunManager.Instance != null && RunManager.Instance.IsFishingRoundActive())
        {
            RunManager.Instance.RecordFishCatch(fish);
        }
        else if (interactor != null)
        {
            if (interactor.inventory != null)
            {
                interactor.inventory.AddFish(fish, !awardCoinsImmediately);
            }

            if (awardCoinsImmediately && interactor.currency != null)
            {
                interactor.currency.AddCoins(fish.sellValue);
                coinsGained = fish.sellValue;
            }

            if (interactor.progression != null)
            {
                levelsGained = interactor.progression.AddXP(fish.xpReward);
            }
        }
        else
        {
            Debug.Log("Caught " + fish.GetDisplayName() + ", but no inventory was assigned.");
        }

        ShowFishCatch(interactor, fish, coinsGained, xpGained, levelsGained, isNewCatch);
    }

    private FishData ApplyCatchQuality(FishData fish, CatchQuality quality)
    {
        fish.catchQuality = quality;

        float multiplier = GetQualityMultiplier(quality);
        fish.weight *= multiplier;
        fish.magicPower = Mathf.RoundToInt(fish.magicPower * multiplier);
        fish.sellValue = Mathf.RoundToInt(fish.sellValue * multiplier);
        fish.xpReward = Mathf.RoundToInt(fish.xpReward * multiplier);

        if (quality == CatchQuality.Perfect && Random.value < 0.5f)
        {
            fish.specialTrait = "Perfect " + fish.specialTrait;
        }
        else if (quality == CatchQuality.Great && Random.value < 0.25f)
        {
            fish.specialTrait = "Polished " + fish.specialTrait;
        }

        return fish;
    }

    private FishData ApplyWizardRewards(PlayerInteractor interactor, FishData fish)
    {
        int wisdom = GetTotalStat(interactor, WizardStatType.Wisdom);
        float sellBonus = wisdom * 0.01f;
        float xpBonus = wisdom * 0.015f;

        if (interactor != null && interactor.wizardGear != null)
        {
            sellBonus += interactor.wizardGear.GetAffixValue(GearAffixType.SellValuePercent) / 100f;
            xpBonus += interactor.wizardGear.GetAffixValue(GearAffixType.FishXPPercent) / 100f;
        }

        fish.sellValue = Mathf.RoundToInt(fish.sellValue * (1f + sellBonus));
        fish.xpReward = Mathf.RoundToInt(fish.xpReward * (1f + xpBonus));
        return fish;
    }

    private float GetLuckRarityBonus(PlayerInteractor interactor)
    {
        int luck = GetTotalStat(interactor, WizardStatType.Luck);
        float bonus = luck * 0.35f;

        if (interactor != null && interactor.wizardGear != null)
        {
            bonus += interactor.wizardGear.GetAffixValue(GearAffixType.RareFishChancePercent) * 0.1f;
        }

        return bonus;
    }

    private float GetSpecialTraitChance(PlayerInteractor interactor)
    {
        int luck = GetTotalStat(interactor, WizardStatType.Luck);
        return Mathf.Clamp01(0.35f + luck * 0.01f);
    }

    private int GetTotalStat(PlayerInteractor interactor, WizardStatType statType)
    {
        if (interactor == null)
        {
            return 0;
        }

        int total = interactor.wizardStats != null ? interactor.wizardStats.GetStat(statType) : 0;
        total += interactor.wizardGear != null ? interactor.wizardGear.GetStatBonus(statType) : 0;
        return total;
    }

    private float GetQualityMultiplier(CatchQuality quality)
    {
        switch (quality)
        {
            case CatchQuality.Perfect:
                return 1.5f;
            case CatchQuality.Great:
                return 1.25f;
            case CatchQuality.Good:
                return 1f;
            case CatchQuality.Poor:
                return 0.65f;
            default:
                return 0f;
        }
    }

    private FishRaritySettings ChooseRaritySettings(FishingRodData rod, float runRarityBonus)
    {
        if (raritySettings == null || raritySettings.Length == 0)
        {
            return new FishRaritySettings();
        }

        float rarityBonus = (rod != null ? rod.rarityBonus : 0f) + runRarityBonus;
        float totalWeight = 0f;
        for (int i = 0; i < raritySettings.Length; i++)
        {
            totalWeight += GetEffectiveRarityWeight(raritySettings[i], i, rarityBonus);
        }

        float roll = Random.Range(0f, totalWeight);
        for (int i = 0; i < raritySettings.Length; i++)
        {
            FishRaritySettings settings = raritySettings[i];
            roll -= GetEffectiveRarityWeight(settings, i, rarityBonus);
            if (roll <= 0f)
            {
                return settings;
            }
        }

        return raritySettings[raritySettings.Length - 1];
    }

    private string ChooseRandomText(string[] options, string fallback)
    {
        if (options == null || options.Length == 0)
        {
            return fallback;
        }

        return options[Random.Range(0, options.Length)];
    }

    private float GetEffectiveRarityWeight(FishRaritySettings settings, int rarityIndex, float rarityBonus)
    {
        float bonus = rarityIndex > 0 ? rarityIndex * rarityBonus : 0f;
        float commonReduction = rarityIndex == 0 ? rarityBonus * 1.5f : 0f;
        return Mathf.Max(0.01f, settings.rollWeight + bonus - commonReduction);
    }

    private void ShowFishCatch(PlayerInteractor interactor, FishData fish, int coinsGained, int xpGained, int levelsGained, bool isNewCatch)
    {
        if (interactor != null)
        {
            interactor.ShowCatchReward(fish, coinsGained, xpGained, levelsGained, isNewCatch);
        }
        else
        {
            Debug.Log("You caught a " + fish.GetDisplayName() + "!");
        }
    }

    private Vector2 SortRange(Vector2 range)
    {
        if (range.x > range.y)
        {
            return new Vector2(range.y, range.x);
        }

        return range;
    }

    private Vector2Int SortRange(Vector2Int range)
    {
        if (range.x > range.y)
        {
            return new Vector2Int(range.y, range.x);
        }

        return range;
    }
}
