using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Legacy experiment from the first fishing prototype.
// The main Wizard Pond loop now uses PlayerInteractor + FishingSpotInteractable.
public class FirstPersonFishingController : MonoBehaviour
{
    private enum FishingState
    {
        Ready,
        WaitingForBite,
        Bite,
        Reeling
    }

    [Header("References")]
    public Camera playerCamera;
    public Transform rodTip;
    public FishingBobber bobberPrefab;
    public LineRenderer fishingLine;
    public Text statusText;

    [Header("Legacy Alternate Casting")]
    public bool legacyCastingEnabled = false;
    public bool showLegacyStartupHint = false;

    [Header("Casting")]
    public KeyCode castKey = KeyCode.Mouse0;
    public KeyCode reelKey = KeyCode.E;
    public KeyCode cancelKey = KeyCode.Mouse1;
    [Min(1f)] public float castDistance = 20f;
    public LayerMask fishableLayers = ~0;
    public float bobberWaterOffset = 0.05f;

    [Header("Reeling")]
    [Min(0.1f)] public float baseReelSpeed = 1f;
    [Min(0.1f)] public float fishEscapeSeconds = 4f;

    private FishingState state = FishingState.Ready;
    private FishingBobber activeBobber;
    private FishingSpot activeSpot;
    private FishDefinition hookedFish;
    private Coroutine biteRoutine;
    private float reelProgress;
    private float escapeTimer;

    private void Awake()
    {
        if (!legacyCastingEnabled)
        {
            enabled = false;
            Debug.Log("FirstPersonFishingController is a legacy alternate left-click casting system and is disabled. Starter Pond uses Starter Fishing Spot + E.");
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (fishingLine != null)
        {
            fishingLine.positionCount = 0;
        }

        if (showLegacyStartupHint)
        {
            SetStatus("Legacy alternate casting enabled: aim at fishable water and left-click to cast.");
        }
    }

    private void Update()
    {
        UpdateFishingLine();

        if (Input.GetKeyDown(cancelKey))
        {
            CancelFishing("Cast cancelled.");
            return;
        }

        switch (state)
        {
            case FishingState.Ready:
                if (Input.GetKeyDown(castKey))
                {
                    TryCast();
                }
                break;

            case FishingState.Bite:
                escapeTimer -= Time.deltaTime;
                SetStatus("Bite! Hold E to reel.");

                if (Input.GetKey(reelKey))
                {
                    StartReeling();
                }
                else if (escapeTimer <= 0f)
                {
                    CancelFishing("The fish got away.");
                }
                break;

            case FishingState.Reeling:
                ReelFish();
                break;
        }
    }

    private void TryCast()
    {
        if (playerCamera == null || bobberPrefab == null)
        {
            SetStatus("Fishing setup is missing a camera or bobber prefab.");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, castDistance, fishableLayers, QueryTriggerInteraction.Ignore))
        {
            SetStatus("No fishable water in range.");
            return;
        }

        FishingSpot spot = hit.collider.GetComponentInParent<FishingSpot>();
        if (spot == null)
        {
            SetStatus("That surface needs a FishingSpot component.");
            return;
        }

        activeSpot = spot;
        Vector3 bobberPosition = hit.point + Vector3.up * bobberWaterOffset;
        activeBobber = Instantiate(bobberPrefab, bobberPosition, Quaternion.identity);
        activeBobber.LandAt(bobberPosition);

        if (fishingLine != null)
        {
            fishingLine.positionCount = 2;
        }

        state = FishingState.WaitingForBite;
        SetStatus("Waiting for a bite...");
        biteRoutine = StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(activeSpot.GetBiteDelay());

        hookedFish = activeSpot.ChooseFish();
        escapeTimer = fishEscapeSeconds;
        reelProgress = 0f;
        state = FishingState.Bite;

        if (activeBobber != null)
        {
            activeBobber.SetBite(true);
        }
    }

    private void StartReeling()
    {
        state = FishingState.Reeling;
        SetStatus("Reeling...");
    }

    private void ReelFish()
    {
        if (hookedFish == null)
        {
            CancelFishing("Nothing was hooked.");
            return;
        }

        if (!Input.GetKey(reelKey))
        {
            state = FishingState.Bite;
            SetStatus("Keep holding E or the fish may escape.");
            return;
        }

        float difficulty = Mathf.Max(0.1f, hookedFish.reelDifficulty);
        float targetSeconds = Mathf.Max(0.1f, hookedFish.reelSeconds);
        reelProgress += Time.deltaTime * baseReelSpeed / (targetSeconds * difficulty);

        SetStatus("Reeling " + hookedFish.fishName + "... " + Mathf.RoundToInt(reelProgress * 100f) + "%");

        if (reelProgress >= 1f)
        {
            string fishName = hookedFish.fishName;
            ClearActiveCast();
            state = FishingState.Ready;
            SetStatus("Caught a " + fishName + "! Legacy casting: left-click to cast again.");
        }
    }

    private void CancelFishing(string message)
    {
        ClearActiveCast();
        state = FishingState.Ready;
        SetStatus(message + " Legacy casting: left-click to cast.");
    }

    private void ClearActiveCast()
    {
        if (biteRoutine != null)
        {
            StopCoroutine(biteRoutine);
            biteRoutine = null;
        }

        if (activeBobber != null)
        {
            Destroy(activeBobber.gameObject);
            activeBobber = null;
        }

        activeSpot = null;
        hookedFish = null;
        reelProgress = 0f;
        escapeTimer = 0f;

        if (fishingLine != null)
        {
            fishingLine.positionCount = 0;
        }
    }

    private void UpdateFishingLine()
    {
        if (fishingLine == null || activeBobber == null || rodTip == null)
        {
            return;
        }

        fishingLine.SetPosition(0, rodTip.position);
        fishingLine.SetPosition(1, activeBobber.transform.position);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        Debug.Log(message);
    }
}
