using System.Collections.Generic;
using UnityEngine;

// Owns the roguelite run state machine: lobby, timed rounds, boss, and summary.
// It tracks temporary run fish, then moves them into permanent inventory at run end.
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Run Settings")]
    public int normalRoundCount = 3;
    public float roundDuration = 45f;
    public float rarityBonusPerRound = 2f;
    public int roundCompleteXP = 25;
    public int bossEscapeXP = 30;
    public KeyCode abandonRunKey = KeyCode.X;

    [Header("References")]
    public PrototypeHud prototypeHud;
    public PlayerCurrency playerCurrency;
    public PlayerProgression playerProgression;
    public SimpleInventory playerInventory;
    public BossFishEncounter bossEncounter;

    public RunState CurrentState { get; private set; } = RunState.Lobby;
    public int CurrentRound { get; private set; }
    public float TimeRemaining { get; private set; }
    public int FishCaughtThisRun => runFish.Count;
    public int FishCaughtThisRound { get; private set; }
    public int RunFishValue { get; private set; }
    public int RunXPEarned { get; private set; }
    public int RunGoldEarned { get; private set; }
    public int RunLevelUpsGained { get; private set; }
    public string RunGearDrops { get; private set; } = "None";
    public string BossResult { get; private set; } = "Not attempted";

    private readonly List<FishData> runFish = new List<FishData>();
    private FishData rarestCatch;
    private bool hasRarestCatch;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (prototypeHud == null)
        {
            prototypeHud = FindAnyObjectByType<PrototypeHud>();
        }

        if (playerCurrency == null)
        {
            playerCurrency = FindAnyObjectByType<PlayerCurrency>();
        }

        if (playerProgression == null)
        {
            playerProgression = FindAnyObjectByType<PlayerProgression>();
        }

        if (playerInventory == null)
        {
            playerInventory = FindAnyObjectByType<SimpleInventory>();
        }

        if (bossEncounter == null)
        {
            bossEncounter = FindAnyObjectByType<BossFishEncounter>();
        }
    }

    private void Start()
    {
        SetState(RunState.Lobby);
    }

    private void Update()
    {
        if (CanAbandonRun() && Input.GetKeyDown(abandonRunKey))
        {
            AbandonRun();
        }
        else if (CurrentState == RunState.FishingRound)
        {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f)
            {
                EndCurrentRound();
            }
        }
        else if (CurrentState == RunState.RoundResults && Input.GetKeyDown(KeyCode.C))
        {
            ContinueFromRoundResults();
        }
        else if (CurrentState == RunState.RunComplete && Input.GetKeyDown(KeyCode.L))
        {
            ReturnToLobby();
        }

        UpdateHud();
    }

    public void StartRun()
    {
        if (CurrentState != RunState.Lobby && CurrentState != RunState.RunComplete)
        {
            return;
        }

        runFish.Clear();
        CurrentRound = 0;
        FishCaughtThisRound = 0;
        RunFishValue = 0;
        RunXPEarned = 0;
        RunGoldEarned = 0;
        RunLevelUpsGained = 0;
        RunGearDrops = "None";
        BossResult = "Not attempted";
        hasRarestCatch = false;
        StartNextRound();
    }

    public bool IsFishingRoundActive()
    {
        return CurrentState == RunState.FishingRound;
    }

    public float GetRunRarityBonus()
    {
        if (CurrentState != RunState.FishingRound)
        {
            return 0f;
        }

        return Mathf.Max(0, CurrentRound - 1) * rarityBonusPerRound;
    }

    public void RecordFishCatch(FishData fish)
    {
        if (CurrentState != RunState.FishingRound)
        {
            return;
        }

        runFish.Add(fish);
        FishCaughtThisRound++;
        RunFishValue += fish.sellValue;

        int xp = fish.xpReward > 0 ? fish.xpReward : GetXPForFish(fish);
        RunXPEarned += xp;
        if (playerProgression != null)
        {
            RunLevelUpsGained += playerProgression.AddXP(xp);
        }

        if (!hasRarestCatch || fish.rarity > rarestCatch.rarity)
        {
            rarestCatch = fish;
            hasRarestCatch = true;
        }

        UpdateHud();
    }

    public void StartBossEncounter()
    {
        SetState(RunState.BossEncounter);

        if (bossEncounter != null)
        {
            bossEncounter.SetEncounterAvailable(true);
        }
    }

    public void CompleteBossEncounter(BossEncounterResult result)
    {
        BossResult = result.caught ? "Caught " + result.bossName : result.bossName + " escaped";
        RunGoldEarned += result.goldEarned;
        if (playerCurrency != null && result.goldEarned > 0)
        {
            playerCurrency.AddGold(result.goldEarned);
        }

        int xp = result.xpEarned > 0 ? result.xpEarned : bossEscapeXP;
        RunXPEarned += xp;
        if (playerProgression != null)
        {
            RunLevelUpsGained += playerProgression.AddXP(xp);
        }

        if (result.gearDropped)
        {
            RunGearDrops = string.IsNullOrEmpty(result.gearDropName) ? "Boss gear drop" : result.gearDropName;
        }

        if (result.caught && playerInventory != null && !string.IsNullOrEmpty(result.bossFishItemName))
        {
            playerInventory.AddItem(result.bossFishItemName);
        }

        CompleteRun();
    }

    public void AbandonRun()
    {
        if (!CanAbandonRun())
        {
            return;
        }

        BossResult = "Run abandoned";

        if (bossEncounter != null)
        {
            bossEncounter.SetEncounterAvailable(false);
        }

        if (playerInventory != null)
        {
            playerInventory.AddFishRange(runFish);
        }

        runFish.Clear();
        TimeRemaining = 0f;
        FishCaughtThisRound = 0;
        RunFishValue = 0;
        RunXPEarned = 0;
        SetState(RunState.Lobby);

        if (prototypeHud != null)
        {
            prototypeHud.SetStatus("Run abandoned. Current run fish were moved to inventory.");
        }
    }

    private void StartNextRound()
    {
        CurrentRound++;
        FishCaughtThisRound = 0;
        TimeRemaining = roundDuration;
        SetState(RunState.FishingRound);
    }

    private void EndCurrentRound()
    {
        TimeRemaining = 0f;

        RunXPEarned += roundCompleteXP;
        if (playerProgression != null)
        {
            RunLevelUpsGained += playerProgression.AddXP(roundCompleteXP);
        }

        SetState(RunState.RoundResults);
    }

    private void ContinueFromRoundResults()
    {
        if (CurrentRound < normalRoundCount)
        {
            StartNextRound();
        }
        else
        {
            StartBossEncounter();
        }
    }

    private void CompleteRun()
    {
        if (playerInventory != null)
        {
            playerInventory.AddFishRange(runFish);
        }

        SetState(RunState.RunComplete);
    }

    private void ReturnToLobby()
    {
        SetState(RunState.Lobby);
    }

    private bool CanAbandonRun()
    {
        return CurrentState == RunState.FishingRound
            || CurrentState == RunState.RoundResults
            || CurrentState == RunState.BossEncounter;
    }

    private void SetState(RunState newState)
    {
        CurrentState = newState;

        if (prototypeHud != null)
        {
            prototypeHud.SetRunState(newState.ToString());
        }

        UpdateHud();
    }

    private void UpdateHud()
    {
        if (prototypeHud == null)
        {
            return;
        }

        prototypeHud.SetRunInfo(
            CurrentState.ToString(),
            TimeRemaining,
            CurrentRound,
            FishCaughtThisRun,
            FishCaughtThisRound,
            RunFishValue,
            RunXPEarned);

        if (CurrentState == RunState.RoundResults)
        {
            prototypeHud.SetStatus("Round " + CurrentRound + " complete. Fish this round: " + FishCaughtThisRound + ". Press C to continue.");
        }
        else if (CurrentState == RunState.BossEncounter)
        {
            prototypeHud.SetStatus("Boss encounter ready. Interact with the boss fishing spot.");
        }
        else if (CurrentState == RunState.RunComplete)
        {
            prototypeHud.SetStatus(GetRunSummary());
        }
    }

    private string GetRunSummary()
    {
        string rarest = hasRarestCatch ? rarestCatch.GetDisplayName() : "None";
        return "Run Complete!\n"
            + "Fish Caught: " + FishCaughtThisRun + "\n"
            + "Rarest Catch: " + rarest + "\n"
            + "Boss Result: " + BossResult + "\n"
            + "Fish Value Banked: " + RunFishValue + " gold\n"
            + "Gold Earned: " + RunGoldEarned + "\n"
            + "Gear Drops: " + RunGearDrops + "\n"
            + "XP Earned: " + RunXPEarned + "\n"
            + "Level Ups: " + RunLevelUpsGained + "\n"
            + "Fish moved to inventory. Sell them in the village shop.\n"
            + "Press L to return to lobby.";
    }

    private int GetXPForFish(FishData fish)
    {
        switch (fish.rarity)
        {
            case FishRarity.Uncommon:
                return 8;
            case FishRarity.Rare:
                return 15;
            case FishRarity.Epic:
                return 30;
            case FishRarity.Legendary:
                return 60;
            case FishRarity.Mythic:
                return 120;
            default:
                return 5;
        }
    }
}
