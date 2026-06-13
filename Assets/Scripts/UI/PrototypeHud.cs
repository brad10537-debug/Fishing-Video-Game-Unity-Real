using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays prototype information only. Gameplay decisions live in the other systems.
public class PrototypeHud : MonoBehaviour
{
    [Header("Text")]
    public Text promptText;
    public Text statusText;
    public Text inventoryText;
    public Text goldText;
    public Text equippedRodText;
    public Text lastCatchText;
    public Text fishCountText;
    public Text shopMenuText;
    public Text runStateText;
    public Text runTimerText;
    public Text runRoundText;
    public Text runFishCountText;
    public Text runValueText;
    public Text playerLevelText;
    public Text xpText;
    public Text bossStatusText;
    public Text minigameTitleText;
    public Text minigameInstructionsText;
    public Text minigamePromptText;
    public Text minigameProgressText;
    public Text minigameResultText;
    public Text wizardStatsText;
    public Text gearText;
    public Text lookTargetText;
    public Text targetHealthText;
    public Text currentAreaText;
    public Text playerHealthText;
    public Text playerStaminaText;
    public Text movementStateText;
    public Text popupText;
    public Slider targetHealthSlider;
    public Slider playerHealthSlider;
    public Slider playerStaminaSlider;
    public Slider minigameProgressSlider;
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupRectTransform;

    [Header("Popup Animation")]
    public float popupVisibleSeconds = 3.5f;
    public float popupFadeSeconds = 0.25f;
    public float popupPulseScale = 1.12f;

    [Header("References")]
    public SimpleInventory inventory;
    public PlayerCurrency currency;
    public PlayerFishingRod fishingRod;
    public PlayerProgression progression;
    public WizardStats wizardStats;
    public WizardGear wizardGear;
    public LivingEntity playerEntity;

    private Coroutine popupRoutine;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<SimpleInventory>();
        }

        if (currency == null)
        {
            currency = FindAnyObjectByType<PlayerCurrency>();
        }

        if (fishingRod == null)
        {
            fishingRod = FindAnyObjectByType<PlayerFishingRod>();
        }

        if (progression == null)
        {
            progression = FindAnyObjectByType<PlayerProgression>();
        }

        if (wizardStats == null)
        {
            wizardStats = FindAnyObjectByType<WizardStats>();
        }

        if (wizardGear == null)
        {
            wizardGear = FindAnyObjectByType<WizardGear>();
        }

        if (playerEntity == null)
        {
            playerEntity = FindPlayerEntity();
        }
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged += UpdateInventory;
            UpdateInventory();
        }

        if (currency != null)
        {
            currency.GoldChanged += UpdateGold;
            UpdateGold(currency.gold);
        }

        if (fishingRod != null)
        {
            fishingRod.EquippedRodChanged += UpdateEquippedRod;
            UpdateEquippedRod(fishingRod.GetEquippedRod());
            fishingRod.RodLevelChanged += UpdateRodLevel;
            UpdateRodLevel(fishingRod.rodLevel);
        }

        if (progression != null)
        {
            progression.ProgressionChanged += UpdateProgression;
            UpdateProgression(progression.level, progression.currentXP, progression.xpToNextLevel);
        }

        if (wizardStats != null)
        {
            wizardStats.StatsChanged += UpdateWizardStats;
            UpdateWizardStats();
        }

        if (wizardGear != null)
        {
            wizardGear.GearChanged += UpdateGear;
            UpdateGear();
        }

        if (playerEntity != null)
        {
            playerEntity.EntityChanged += UpdatePlayerVitals;
            UpdatePlayerVitals(playerEntity);
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged -= UpdateInventory;
        }

        if (currency != null)
        {
            currency.GoldChanged -= UpdateGold;
        }

        if (fishingRod != null)
        {
            fishingRod.EquippedRodChanged -= UpdateEquippedRod;
            fishingRod.RodLevelChanged -= UpdateRodLevel;
        }

        if (progression != null)
        {
            progression.ProgressionChanged -= UpdateProgression;
        }

        if (wizardStats != null)
        {
            wizardStats.StatsChanged -= UpdateWizardStats;
        }

        if (wizardGear != null)
        {
            wizardGear.GearChanged -= UpdateGear;
        }

        if (playerEntity != null)
        {
            playerEntity.EntityChanged -= UpdatePlayerVitals;
        }
    }

    public void SetPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    public void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void ShowCaughtFish(FishData fish)
    {
        string message = fish.GetCatchDetails();

        SetStatus(message);

        if (lastCatchText != null)
        {
            lastCatchText.text = "Last Catch:\n" + message;
        }
    }

    public void ShowCatchReward(FishData fish, int coinsGained, int xpGained, int levelsGained, bool isNewCatch)
    {
        string levelMessage = levelsGained > 0
            ? "\nLEVEL UP! New Level: " + (progression != null ? progression.level.ToString() : "+1")
            : "";
        string newCatchMessage = isNewCatch ? "\nNew Journal Entry!" : "";

        string message =
            "Caught: " + fish.GetDisplayName() + "\n"
            + "Rarity: " + fish.rarity + "\n"
            + "Weight: " + fish.weight.ToString("0.0") + " lbs\n"
            + "Behavior: " + fish.GetBehaviorDisplayName() + "\n"
            + "Coins Gained: +" + coinsGained + "\n"
            + "XP Gained: +" + xpGained
            + newCatchMessage
            + levelMessage;

        SetStatus(message);

        if (lastCatchText != null)
        {
            lastCatchText.text = "Last Catch:\n" + fish.GetDisplayName() + "\n"
                + fish.rarity + " | " + fish.weight.ToString("0.0") + " lbs\n"
                + "Behavior: " + fish.GetBehaviorDisplayName() + "\n"
                + "+" + coinsGained + " coins | +" + xpGained + " XP";
        }

        if (minigameResultText != null)
        {
            minigameResultText.text = "Caught " + fish.fishName + "! +" + coinsGained + " coins, +" + xpGained + " XP.";
        }

        string popup =
            "Caught!\n"
            + fish.fishName + "\n"
            + fish.rarity + " - " + fish.weight.ToString("0.0") + " lbs\n"
            + "Behavior: " + fish.GetBehaviorDisplayName() + "\n"
            + "+" + coinsGained + " Coins - +" + xpGained + " XP";

        if (isNewCatch)
        {
            popup += "\nNew Journal Entry!";
        }

        if (levelsGained > 0)
        {
            popup += "\nLevel Up!";
        }

        PopupStyle style = GetCatchPopupStyle(fish.rarity, levelsGained > 0);
        ShowPopup(popup, style.color, true, style.visibleSeconds, style.pulseScale);
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayCatchReward(fish.rarity);
        }
    }

    public void ShowFishingFailure(string reason)
    {
        string message = "The fish got away!";
        if (!string.IsNullOrEmpty(reason))
        {
            message += "\n" + reason;
        }

        SetStatus(message);

        if (minigameResultText != null)
        {
            minigameResultText.text = message;
        }

        ShowPopup(message, new Color(1f, 0.65f, 0.65f), true, 3f, 1.05f);
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayMinigameFailure();
        }
    }

    public void ShowShopPopup(string message, bool success)
    {
        SetStatus(message);
        ShowPopup(message, success ? new Color(1f, 0.9f, 0.45f) : new Color(1f, 0.7f, 0.55f), success, success ? 3.5f : 2.5f, success ? 1.18f : 1.06f);
        if (!success && GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayShopDenied();
        }
    }

    public void SetShopMenu(string message)
    {
        if (shopMenuText != null)
        {
            shopMenuText.text = message;
        }
        else if (statusText != null && !string.IsNullOrEmpty(message))
        {
            statusText.text = message;
        }
    }

    public void SetRunState(string stateName)
    {
        if (runStateText != null)
        {
            runStateText.text = "State: " + stateName;
        }
    }

    public void SetRunInfo(string stateName, float timer, int roundNumber, int runFishCount, int roundFishCount, int runValue, int runXP)
    {
        SetRunState(stateName);

        if (runTimerText != null)
        {
            runTimerText.text = "Timer: " + Mathf.CeilToInt(Mathf.Max(0f, timer)) + "s";
        }

        if (runRoundText != null)
        {
            runRoundText.text = "Round: " + roundNumber;
        }

        if (runFishCountText != null)
        {
            runFishCountText.text = "Run Fish: " + runFishCount + " (Round: " + roundFishCount + ")";
        }

        if (runValueText != null)
        {
            runValueText.text = "Run Fish Value: " + runValue + " coins | XP: " + runXP;
        }
    }

    public void SetBossStatus(string message)
    {
        if (bossStatusText != null)
        {
            bossStatusText.text = message;
        }
        else if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void SetLookTarget(string message)
    {
        if (lookTargetText != null)
        {
            lookTargetText.text = message;
        }
    }

    public void SetTargetEntity(CharacterIdentity identity, LivingEntity entity)
    {
        bool hasTarget = identity != null && entity != null && IsEnemyOrBoss(identity);

        if (targetHealthText != null)
        {
            targetHealthText.text = hasTarget
                ? identity.displayName + " HP: " + Mathf.CeilToInt(entity.health) + " / " + Mathf.CeilToInt(entity.maxHealth)
                : "";
        }

        if (targetHealthSlider != null)
        {
            targetHealthSlider.gameObject.SetActive(hasTarget);
            targetHealthSlider.value = hasTarget ? entity.GetHealthPercent() : 0f;
        }
    }

    public void SetCurrentArea(string areaName)
    {
        if (currentAreaText != null)
        {
            currentAreaText.text = string.IsNullOrEmpty(areaName) ? "" : "Area: " + areaName;
        }
    }

    public void SetMovementState(string stateName)
    {
        if (movementStateText != null)
        {
            movementStateText.text = "Movement: " + stateName;
        }
    }

    public void SetFishingMinigame(string title, string instructions, float timeLeft, float progress, string prompt, int currentScore, int targetScore, string extra)
    {
        if (minigameTitleText != null)
        {
            minigameTitleText.text = title;
        }

        if (minigameInstructionsText != null)
        {
            minigameInstructionsText.text = instructions;
        }

        if (minigamePromptText != null)
        {
            minigamePromptText.text = prompt;
        }

        if (minigameProgressText != null)
        {
            minigameProgressText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0f, timeLeft)) + "s | Progress: " + currentScore + " / " + targetScore + "\n" + extra;
        }

        if (minigameProgressSlider != null)
        {
            minigameProgressSlider.value = Mathf.Clamp01(progress);
        }

        if (minigameResultText != null && title == "Result")
        {
            minigameResultText.text = instructions;
        }
    }

    private void ShowPopup(string message, Color color, bool pulse)
    {
        ShowPopup(message, color, pulse, popupVisibleSeconds, popupPulseScale);
    }

    private void ShowPopup(string message, Color color, bool pulse, float visibleSeconds, float pulseScale)
    {
        if (popupText == null)
        {
            SetStatus(message);
            return;
        }

        popupText.text = message;
        popupText.color = color;

        if (popupCanvasGroup == null)
        {
            popupCanvasGroup = popupText.GetComponent<CanvasGroup>();
        }

        if (popupRectTransform == null)
        {
            popupRectTransform = popupText.GetComponent<RectTransform>();
        }

        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        popupRoutine = StartCoroutine(AnimatePopup(pulse, visibleSeconds, pulseScale));
    }

    private IEnumerator AnimatePopup(bool pulse, float visibleSeconds, float pulseScale)
    {
        if (popupCanvasGroup == null)
        {
            yield break;
        }

        if (popupRectTransform != null)
        {
            popupRectTransform.localScale = Vector3.one * (pulse ? pulseScale : 1f);
        }

        yield return FadePopup(0f, 1f, popupFadeSeconds);

        if (popupRectTransform != null && pulse)
        {
            float timer = 0f;
            while (timer < 0.25f)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / 0.25f);
                popupRectTransform.localScale = Vector3.Lerp(Vector3.one * pulseScale, Vector3.one, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0.1f, visibleSeconds));
        yield return FadePopup(1f, 0f, popupFadeSeconds);
        popupRoutine = null;
    }

    private PopupStyle GetCatchPopupStyle(FishRarity rarity, bool leveledUp)
    {
        PopupStyle style = new PopupStyle
        {
            color = new Color(0.7f, 1f, 0.85f),
            visibleSeconds = popupVisibleSeconds,
            pulseScale = popupPulseScale
        };

        switch (rarity)
        {
            case FishRarity.Mythic:
                style.color = new Color(1f, 0.45f, 1f);
                style.visibleSeconds = 5f;
                style.pulseScale = 1.35f;
                break;
            case FishRarity.Legendary:
                style.color = new Color(1f, 0.75f, 0.25f);
                style.visibleSeconds = 4.8f;
                style.pulseScale = 1.3f;
                break;
            case FishRarity.Epic:
                style.color = new Color(0.95f, 0.45f, 1f);
                style.visibleSeconds = 4.4f;
                style.pulseScale = 1.24f;
                break;
            case FishRarity.Rare:
                style.color = new Color(0.4f, 0.9f, 1f);
                style.visibleSeconds = 4f;
                style.pulseScale = 1.18f;
                break;
            case FishRarity.Uncommon:
                style.color = new Color(0.55f, 1f, 0.55f);
                style.visibleSeconds = 3.7f;
                style.pulseScale = 1.14f;
                break;
        }

        if (leveledUp)
        {
            style.visibleSeconds += 0.8f;
            style.pulseScale += 0.08f;
        }

        return style;
    }

    private struct PopupStyle
    {
        public Color color;
        public float visibleSeconds;
        public float pulseScale;
    }

    private IEnumerator FadePopup(float from, float to, float seconds)
    {
        float timer = 0f;
        while (timer < seconds)
        {
            timer += Time.deltaTime;
            float t = seconds <= 0f ? 1f : Mathf.Clamp01(timer / seconds);
            popupCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        popupCanvasGroup.alpha = to;
    }

    private void UpdateInventory()
    {
        if (inventory == null)
        {
            if (inventoryText != null)
            {
                inventoryText.text = "Inventory: Empty";
            }

            UpdateFishCount(0);
            return;
        }

        if (inventoryText != null)
        {
            inventoryText.text = inventory.GetInventorySummary();
        }

        UpdateFishCount(inventory.GetFishCount());
    }

    private void UpdateGold(int gold)
    {
        if (goldText != null)
        {
            goldText.text = "Coins: " + gold;
        }
    }

    private void UpdateEquippedRod(FishingRodData rod)
    {
        if (equippedRodText != null && rod != null)
        {
            int rodLevel = fishingRod != null ? fishingRod.rodLevel : 1;
            equippedRodText.text = "Rod: Lv." + rodLevel + " " + rod.rodName;
        }
    }

    private void UpdateRodLevel(int rodLevel)
    {
        UpdateEquippedRod(fishingRod != null ? fishingRod.GetEquippedRod() : null);
    }

    private void UpdateFishCount(int count)
    {
        if (fishCountText != null)
        {
            fishCountText.text = "Fish: " + count;
        }
    }

    private void UpdateProgression(int level, int currentXP, int xpToNextLevel)
    {
        if (playerLevelText != null)
        {
            playerLevelText.text = "Level: " + level;
        }

        if (xpText != null)
        {
            xpText.text = "XP: " + currentXP + " / " + xpToNextLevel;
        }
    }

    private void UpdateWizardStats()
    {
        if (wizardStatsText == null || wizardStats == null)
        {
            return;
        }

        wizardStatsText.text =
            "Stats | Points: " + wizardStats.statPoints + "\n"
            + "F1 Focus: " + wizardStats.focus + "\n"
            + "F2 Reflex: " + wizardStats.reflex + "\n"
            + "F3 Luck: " + wizardStats.luck + "\n"
            + "F4 Control: " + wizardStats.control + "\n"
            + "F5 Power: " + wizardStats.power + "\n"
            + "F6 Wisdom: " + wizardStats.wisdom;
    }

    private void UpdateGear()
    {
        if (gearText != null && wizardGear != null)
        {
            gearText.text = wizardGear.GetGearSummary();
        }
    }

    private void UpdatePlayerVitals(LivingEntity entity)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = entity.GetHealthPercent();
        }

        if (playerStaminaSlider != null)
        {
            playerStaminaSlider.value = entity.GetStaminaPercent();
        }

        if (playerHealthText != null)
        {
            playerHealthText.text = "Health: " + Mathf.CeilToInt(entity.health) + " / " + Mathf.CeilToInt(entity.maxHealth);
        }

        if (playerStaminaText != null)
        {
            playerStaminaText.text = "Stamina: " + Mathf.CeilToInt(entity.stamina) + " / " + Mathf.CeilToInt(entity.maxStamina);
        }
    }

    private LivingEntity FindPlayerEntity()
    {
        LivingEntity[] entities = FindObjectsByType<LivingEntity>(FindObjectsInactive.Exclude);
        foreach (LivingEntity entity in entities)
        {
            CharacterIdentity identity = entity.GetComponent<CharacterIdentity>();
            if (identity != null && identity.characterType == CharacterType.Player)
            {
                return entity;
            }
        }

        return entities.Length > 0 ? entities[0] : null;
    }

    private bool IsEnemyOrBoss(CharacterIdentity identity)
    {
        return identity.characterType == CharacterType.Mob || identity.characterType == CharacterType.Boss;
    }
}
