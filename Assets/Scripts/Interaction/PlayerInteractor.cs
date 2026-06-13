using UnityEngine;

// Owns the center-screen raycast and forwards E presses to IInteractable objects.
// It does not decide what fishing, shops, or loot do after interaction.
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public SimpleInventory inventory;
    public PlayerCurrency currency;
    public PlayerFishingRod fishingRod;
    public PlayerProgression progression;
    public WizardStats wizardStats;
    public WizardGear wizardGear;
    public FishJournal fishJournal;
    public PrototypeHud prototypeHud;
    public FirstPersonPrototypeController movementController;

    [Header("Interaction")]
    public float interactDistance = 4f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactableLayers = ~0;

    private IInteractable currentInteractable;
    private CharacterIdentity currentIdentity;
    private LivingEntity currentTargetEntity;
    private FishingSpotInteractable focusedFishingSpot;
    private string lastPrompt;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (inventory == null)
        {
            inventory = GetComponent<SimpleInventory>();
        }

        if (currency == null)
        {
            currency = GetComponent<PlayerCurrency>();
        }

        if (fishingRod == null)
        {
            fishingRod = GetComponent<PlayerFishingRod>();
        }

        if (progression == null)
        {
            progression = GetComponent<PlayerProgression>();
        }

        if (wizardStats == null)
        {
            wizardStats = GetComponent<WizardStats>();
        }

        if (wizardGear == null)
        {
            wizardGear = GetComponent<WizardGear>();
        }

        if (fishJournal == null)
        {
            fishJournal = GetComponent<FishJournal>();
        }

        if (prototypeHud == null)
        {
            prototypeHud = FindAnyObjectByType<PrototypeHud>();
        }

        if (movementController == null)
        {
            movementController = GetComponent<FirstPersonPrototypeController>();
        }
    }

    private void Update()
    {
        if (movementController != null && movementController.IsSitting())
        {
            if (Input.GetKeyDown(interactKey))
            {
                movementController.ExitSitting();
                ShowStatus("You stand up.");
            }

            return;
        }

        FindInteractable();

        if (currentInteractable != null)
        {
            string prompt = currentInteractable.GetInteractPrompt();
            if (prompt != lastPrompt)
            {
                Debug.Log(prompt);
                if (prototypeHud != null)
                {
                    prototypeHud.SetPrompt(prompt);
                }

                if (GameAudioFeedback.Instance != null)
                {
                    GameAudioFeedback.Instance.PlayInteractionPrompt();
                }

                lastPrompt = prompt;
            }

            if (Input.GetKeyDown(interactKey))
            {
                if (movementController != null)
                {
                    movementController.SetMovementState(PlayerMovementState.Interacting);
                }

                currentInteractable.Interact(this);
            }
        }
        else if (!string.IsNullOrEmpty(lastPrompt))
        {
            lastPrompt = null;
            if (prototypeHud != null)
            {
                prototypeHud.SetPrompt("");
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && inventory != null)
        {
            inventory.PrintInventory();
        }
    }

    public void ShowStatus(string message)
    {
        Debug.Log(message);

        if (prototypeHud != null)
        {
            prototypeHud.SetStatus(message);
        }
    }

    public void ShowCaughtFish(FishData fish)
    {
        Debug.Log(fish.GetCatchDetails());

        if (prototypeHud != null)
        {
            prototypeHud.ShowCaughtFish(fish);
        }
    }

    public void ShowCatchReward(FishData fish, int coinsGained, int xpGained, int levelsGained, bool isNewCatch)
    {
        Debug.Log("Caught " + fish.GetDisplayName() + " | +" + coinsGained + " coins | +" + xpGained + " XP");

        if (prototypeHud != null)
        {
            prototypeHud.ShowCatchReward(fish, coinsGained, xpGained, levelsGained, isNewCatch);
        }
    }

    public void ShowShopPopup(string message, bool success)
    {
        Debug.Log(message);

        if (prototypeHud != null)
        {
            prototypeHud.ShowShopPopup(message, success);
        }
        else
        {
            ShowStatus(message);
        }
    }

    public void ShowFishingFailure(string reason)
    {
        Debug.Log("The fish got away! " + reason);

        if (prototypeHud != null)
        {
            prototypeHud.ShowFishingFailure(reason);
        }
        else
        {
            ShowStatus("The fish got away! " + reason);
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;
        currentIdentity = null;
        currentTargetEntity = null;
        FishingSpotInteractable newFishingSpotFocus = null;

        if (playerCamera == null)
        {
            SetFishingSpotFocus(null);
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayers, QueryTriggerInteraction.Collide))
        {
            if (prototypeHud != null)
            {
                prototypeHud.SetLookTarget("");
                prototypeHud.SetTargetEntity(null, null);
            }
            SetFishingSpotFocus(null);
            return;
        }

        currentIdentity = hit.collider.GetComponentInParent<CharacterIdentity>();
        currentTargetEntity = hit.collider.GetComponentInParent<LivingEntity>();
        if (prototypeHud != null)
        {
            prototypeHud.SetLookTarget(currentIdentity != null ? currentIdentity.GetDisplayText() : "");
            prototypeHud.SetTargetEntity(currentIdentity, currentTargetEntity);
        }

        MonoBehaviour[] behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
            {
                currentInteractable = interactable;
                newFishingSpotFocus = behaviour as FishingSpotInteractable;
                SetFishingSpotFocus(newFishingSpotFocus);
                return;
            }
        }

        SetFishingSpotFocus(null);
    }

    private void SetFishingSpotFocus(FishingSpotInteractable newFocus)
    {
        if (focusedFishingSpot == newFocus)
        {
            return;
        }

        if (focusedFishingSpot != null)
        {
            focusedFishingSpot.SetFocused(false);
        }

        focusedFishingSpot = newFocus;
        if (focusedFishingSpot != null)
        {
            focusedFishingSpot.SetFocused(true);
        }
    }
}
