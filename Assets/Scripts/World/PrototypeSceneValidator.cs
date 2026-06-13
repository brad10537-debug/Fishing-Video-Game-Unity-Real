using UnityEngine;

// Optional setup helper for the prototype scene.
// Add this to an empty GameObject named "Prototype Scene Validator" and press Play.
// It reports missing core objects/components without controlling gameplay.
public class PrototypeSceneValidator : MonoBehaviour
{
    public bool runOnStart = true;
    public bool checkOptionalObjects = true;

    private int warnings;

    private void Start()
    {
        if (runOnStart)
        {
            ValidateScene();
        }
    }

    [ContextMenu("Validate Scene")]
    public void ValidateScene()
    {
        warnings = 0;

        Debug.Log("Wizard Pond scene validation started.");

        ValidatePlayer();
        ValidateHud();
        ValidateRunAndFishingSystems();
        ValidateInteractables();

        if (checkOptionalObjects)
        {
            ValidateOptionalVillageObjects();
        }

        if (warnings == 0)
        {
            Debug.Log("Wizard Pond scene validation passed. Ready for Play Mode testing.");
        }
        else
        {
            Debug.LogWarning("Wizard Pond scene validation finished with " + warnings + " warning(s). Check messages above.");
        }
    }

    private void ValidatePlayer()
    {
        FirstPersonPrototypeController player = FindAnyObjectByType<FirstPersonPrototypeController>();
        if (WarnIfMissing(player, "Player with FirstPersonPrototypeController")) return;

        WarnIfMissing(player.GetComponent<CharacterController>(), "Player CharacterController");
        WarnIfMissing(player.GetComponent<PlayerInteractor>(), "PlayerInteractor on Player");
        WarnIfMissing(player.GetComponent<LivingEntity>(), "LivingEntity on Player");
        WarnIfMissing(player.GetComponent<SimpleInventory>(), "SimpleInventory on Player");
        WarnIfMissing(player.GetComponent<PlayerCurrency>(), "PlayerCurrency on Player");
        WarnIfMissing(player.GetComponent<PlayerFishingRod>(), "PlayerFishingRod on Player");
        WarnIfMissing(player.GetComponent<PlayerProgression>(), "PlayerProgression on Player");
        WarnIfMissing(player.GetComponent<WizardStats>(), "WizardStats on Player");
        WarnIfMissing(player.GetComponent<WizardGear>(), "WizardGear on Player");
        WarnIfMissing(player.GetComponent<FishJournal>(), "FishJournal on Player");
        WarnIfMissing(player.GetComponentInChildren<Camera>(), "Player child Camera");
    }

    private void ValidateHud()
    {
        PrototypeHud hud = FindAnyObjectByType<PrototypeHud>();
        if (WarnIfMissing(hud, "PrototypeHud in scene")) return;

        WarnIfMissing(hud.promptText, "HUD promptText");
        WarnIfMissing(hud.statusText, "HUD statusText");
        WarnIfMissing(hud.playerHealthText, "HUD playerHealthText");
        WarnIfMissing(hud.playerStaminaText, "HUD playerStaminaText");
        WarnIfMissing(hud.goldText, "HUD goldText");
        WarnIfMissing(hud.playerLevelText, "HUD playerLevelText");
        WarnIfMissing(hud.xpText, "HUD xpText");
        WarnIfMissing(hud.runTimerText, "HUD runTimerText");
        WarnIfMissing(hud.minigameTitleText, "HUD minigameTitleText");
        WarnIfMissing(hud.minigameInstructionsText, "HUD minigameInstructionsText");
        WarnIfMissing(hud.minigamePromptText, "HUD minigamePromptText");
        WarnIfMissing(hud.minigameProgressText, "HUD minigameProgressText");
        WarnIfMissing(hud.minigameResultText, "HUD minigameResultText / reward popup text");
        WarnIfMissing(hud.minigameProgressSlider, "HUD minigameProgressSlider");
        WarnIfMissing(hud.lastCatchText, "HUD lastCatchText");
        WarnIfMissing(hud.equippedRodText, "HUD equippedRodText");

        FishJournalUi journalUi = FindAnyObjectByType<FishJournalUi>(FindObjectsInactive.Include);
        WarnIfMissing(journalUi, "FishJournalUi on HUD Canvas");
        if (journalUi != null)
        {
            WarnIfMissing(journalUi.journalPanel, "FishJournalUi journalPanel");
            WarnIfMissing(journalUi.journalText, "FishJournalUi journalText");
            WarnIfMissing(journalUi.journal, "FishJournalUi journal reference");
        }
    }

    private void ValidateRunAndFishingSystems()
    {
        WarnIfMissing(FindAnyObjectByType<RunManager>(), "RunManager");
        WarnIfMissing(FindAnyObjectByType<FishingMinigameManager>(), "FishingMinigameManager");
        WarnIfMissing(FindAnyObjectByType<FishingDifficultyManager>(), "FishingDifficultyManager");
        WarnIfMissing(FindAnyObjectByType<FishingRunPortalInteractable>(), "FishingRunPortalInteractable");
        FishingSpotInteractable fishingSpot = FindAnyObjectByType<FishingSpotInteractable>();
        ShopKeeperInteractable shopkeeper = FindAnyObjectByType<ShopKeeperInteractable>();
        WarnIfMissing(fishingSpot, "FishingSpotInteractable");
        WarnIfMissing(shopkeeper, "ShopKeeperInteractable");

        if (fishingSpot != null)
        {
            bool hasInlineFish = fishingSpot.speciesPool != null && fishingSpot.speciesPool.Length > 0;
            bool hasFishAssets = fishingSpot.fishSpeciesAssets != null && fishingSpot.fishSpeciesAssets.Length > 0;
            if (!hasInlineFish && !hasFishAssets)
            {
                AddWarning(fishingSpot.name + " has no fish data. Add starter fish to Species Pool or assign Fish Species Assets.");
            }
        }

        if (shopkeeper != null)
        {
            Collider shopCollider = shopkeeper.GetComponentInChildren<Collider>();
            if (shopCollider == null)
            {
                AddWarning(shopkeeper.name + " needs a Collider so the player can open the upgrade shop.");
            }
        }
    }

    private void ValidateInteractables()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
        int interactableCount = 0;

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable)
            {
                interactableCount++;
                Collider collider = behaviour.GetComponentInChildren<Collider>();
                if (collider == null)
                {
                    AddWarning(behaviour.name + " implements IInteractable but has no Collider on itself or children.");
                }
            }
        }

        if (interactableCount == 0)
        {
            AddWarning("No IInteractable objects found in scene.");
        }
    }

    private void ValidateOptionalVillageObjects()
    {
        WarnIfMissing(FindAnyObjectByType<NPCInteractable>(), "At least one NPCInteractable");
        WarnIfMissing(FindAnyObjectByType<MobController>(), "At least one MobController or spawned mob");
        WarnIfMissing(FindAnyObjectByType<MobSpawnArea>(), "MobSpawnArea");
        WarnIfMissing(FindAnyObjectByType<BossFishEncounter>(), "BossFishEncounter");
        WarnIfMissing(FindAnyObjectByType<VillageAreaMarker>(), "VillageAreaMarker");
    }

    private bool WarnIfMissing(Object value, string label)
    {
        if (value != null)
        {
            return false;
        }

        AddWarning("Missing: " + label);
        return true;
    }

    private void AddWarning(string message)
    {
        warnings++;
        Debug.LogWarning("[Wizard Pond Setup] " + message);
    }
}
