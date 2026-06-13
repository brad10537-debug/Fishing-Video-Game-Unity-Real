using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Local single-player boss encounter controller.
// The phase data/result split keeps this ready for future co-op/network orchestration.
public class BossFishEncounter : MonoBehaviour, IInteractable
{
    [Header("Boss Pool")]
    public List<BossFishData> bossPool = new List<BossFishData>
    {
        new BossFishData
        {
            bossName = "Lord Bubblebeard",
            bossLevel = 1,
            bossRarity = FishRarity.Legendary,
            flavorText = "A bearded bubble baron who hates sloppy tapping.",
            specialCatchMessage = "Lord Bubblebeard bows, defeated but still bubbly.",
            goldReward = 500,
            xpReward = 120,
            gearDropChance = 0.25f,
            phases = new List<BossPhaseData>
            {
                new BossPhaseData { phaseName = "Bubble Rush", encounterType = FishEncounterType.RapidTap, difficulty = 1f, duration = 8f, captureRequired = 32f, tensionIncreasePerSecond = 4f, instructions = "Tap Space to pop bubbles and gain capture." },
                new BossPhaseData { phaseName = "Beard Glimmer", encounterType = FishEncounterType.TimingWindow, difficulty = 1.2f, duration = 8f, captureRequired = 30f, tensionIncreasePerSecond = 5f, instructions = "Press Space inside the safe timing window." },
                new BossPhaseData { phaseName = "Royal Hookset", encounterType = FishEncounterType.PrecisionStop, difficulty = 1.4f, duration = 7f, captureRequired = 38f, tensionIncreasePerSecond = 6f, instructions = "Stop the meter near the center." }
            }
        },
        new BossFishData
        {
            bossName = "The Mossback Ancient",
            bossLevel = 2,
            bossRarity = FishRarity.Legendary,
            flavorText = "A heavy old fish with a garden growing on its back.",
            specialCatchMessage = "The Mossback Ancient finally agrees to be caught.",
            goldReward = 650,
            xpReward = 150,
            gearDropChance = 0.3f,
            phases = new List<BossPhaseData>
            {
                new BossPhaseData { phaseName = "Heavy Pull", encounterType = FishEncounterType.HoldBalance, difficulty = 1.2f, duration = 10f, captureRequired = 35f, tensionIncreasePerSecond = 5f, instructions = "Hold/release Space to keep tension in the safe zone." },
                new BossPhaseData { phaseName = "Rooted Line", encounterType = FishEncounterType.HoldBalance, difficulty = 1.5f, duration = 9f, captureRequired = 35f, tensionIncreasePerSecond = 6f, instructions = "Control the heavy pull. Stay calm." },
                new BossPhaseData { phaseName = "Ancient Nod", encounterType = FishEncounterType.PrecisionStop, difficulty = 1.6f, duration = 7f, captureRequired = 35f, tensionIncreasePerSecond = 6f, instructions = "Stop near center to earn the Ancient's respect." }
            }
        },
        new BossFishData
        {
            bossName = "Neon Leviathan",
            bossLevel = 3,
            bossRarity = FishRarity.Mythic,
            flavorText = "A glowing streak of impossible pond nightlife.",
            specialCatchMessage = "Neon Leviathan turns the whole pond into a victory sign.",
            goldReward = 900,
            xpReward = 220,
            gearDropChance = 0.45f,
            phases = new List<BossPhaseData>
            {
                new BossPhaseData { phaseName = "Neon Pattern", encounterType = FishEncounterType.SequenceInput, difficulty = 1.5f, duration = 9f, captureRequired = 35f, tensionIncreasePerSecond = 6f, instructions = "Press the WASD sequence." },
                new BossPhaseData { phaseName = "Flash Dodge", encounterType = FishEncounterType.ReactionDodge, difficulty = 1.8f, duration = 8f, captureRequired = 35f, tensionIncreasePerSecond = 7f, instructions = "Press the prompted WASD key." },
                new BossPhaseData { phaseName = "Neon Lock", encounterType = FishEncounterType.SequenceInput, difficulty = 2f, duration = 8f, captureRequired = 40f, tensionIncreasePerSecond = 8f, instructions = "Finish the sequence cleanly." }
            }
        },
        new BossFishData
        {
            bossName = "The Crystal Maw",
            bossLevel = 3,
            bossRarity = FishRarity.Mythic,
            flavorText = "A glassy predator that bites reflections first.",
            specialCatchMessage = "The Crystal Maw cracks into a prism of rewards.",
            goldReward = 850,
            xpReward = 210,
            gearDropChance = 0.4f,
            phases = new List<BossPhaseData>
            {
                new BossPhaseData { phaseName = "Prism Stop", encounterType = FishEncounterType.PrecisionStop, difficulty = 1.4f, duration = 8f, captureRequired = 32f, tensionIncreasePerSecond = 5f, instructions = "Stop near the center." },
                new BossPhaseData { phaseName = "Crystal Drag", encounterType = FishEncounterType.HoldBalance, difficulty = 1.7f, duration = 9f, captureRequired = 36f, tensionIncreasePerSecond = 7f, instructions = "Hold/release to stabilize the line." },
                new BossPhaseData { phaseName = "Shatter Window", encounterType = FishEncounterType.TimingWindow, difficulty = 2f, duration = 7f, captureRequired = 40f, tensionIncreasePerSecond = 8f, instructions = "Hit the final timing window." }
            }
        }
    };

    [Header("Meters")]
    public float maxTension = 100f;
    public float maxCapture = 100f;
    public Slider tensionSlider;
    public Slider captureSlider;

    private BossFishData activeBoss;
    private PlayerInteractor activeInteractor;
    private bool encounterAvailable;
    private bool encounterActive;
    private int phaseIndex;
    private float phaseTimer;
    private float tension;
    private float capture;
    private float phaseMeter;
    private float safeStart;
    private float safeEnd;
    private string sequence;
    private int sequenceIndex;
    private KeyCode reactionKey;

    private readonly KeyCode[] sequenceKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    private void Update()
    {
        if (!encounterActive || activeBoss == null)
        {
            return;
        }

        BossPhaseData phase = GetCurrentPhase();
        phaseTimer -= Time.deltaTime;
        tension += GetModifiedTensionIncrease(phase) * Time.deltaTime;
        tension = Mathf.Clamp(tension, 0f, maxTension);

        UpdatePhase(phase);
        UpdateBossHud(phase);

        if (capture >= maxCapture)
        {
            FinishEncounter(true);
        }
        else if (tension >= maxTension || phaseTimer <= 0f)
        {
            FinishEncounter(false);
        }
        else if (capture >= GetPhaseCaptureTarget())
        {
            AdvancePhase();
        }
    }

    public void SetEncounterAvailable(bool available)
    {
        encounterAvailable = available;
        encounterActive = false;

        if (available)
        {
            activeBoss = ChooseBoss();
            ResetMeters();
        }

        gameObject.SetActive(true);
    }

    public string GetInteractPrompt()
    {
        if (!encounterAvailable || activeBoss == null)
        {
            return "The boss fish is not here yet.";
        }

        if (encounterActive)
        {
            return "Boss fight active: " + activeBoss.bossName;
        }

        return "Press E to challenge " + activeBoss.bossName + ".";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!encounterAvailable || activeBoss == null)
        {
            if (interactor != null)
            {
                interactor.ShowStatus("Finish the normal rounds first.");
            }
            return;
        }

        if (encounterActive)
        {
            return;
        }

        activeInteractor = interactor;
        StartEncounter();
    }

    private void StartEncounter()
    {
        encounterActive = true;
        phaseIndex = 0;
        ResetMeters();
        StartPhase(0);
    }

    private void StartPhase(int index)
    {
        phaseIndex = Mathf.Clamp(index, 0, activeBoss.phases.Count - 1);
        BossPhaseData phase = GetCurrentPhase();
        phaseTimer = GetModifiedPhaseDuration(phase);
        phaseMeter = 0f;
        sequenceIndex = 0;
        sequence = BuildSequence(Mathf.Clamp(2 + phaseIndex, 2, 6));
        reactionKey = PickPromptKey();
        float safeSize = GetSafeWindowSize(phase);
        safeStart = Random.Range(0.1f, 0.9f - safeSize);
        safeEnd = safeStart + safeSize;

        if (activeInteractor != null)
        {
            activeInteractor.ShowStatus("Boss phase " + (phaseIndex + 1) + ": " + phase.phaseName);
        }
    }

    private void UpdatePhase(BossPhaseData phase)
    {
        switch (phase.encounterType)
        {
            case FishEncounterType.RapidTap:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AddCapture(GetCapturePower(8f));
                }
                phaseMeter = capture / maxCapture;
                break;
            case FishEncounterType.HoldBalance:
                phaseMeter += (Input.GetKey(KeyCode.Space) ? 1f : -1f) * Time.deltaTime * phase.difficulty * GetControlMultiplier();
                phaseMeter = Mathf.Clamp01(phaseMeter);
                if (phaseMeter >= safeStart && phaseMeter <= safeEnd)
                {
                    AddCapture(GetCapturePower(12f) * Time.deltaTime);
                    tension -= GetControlBonus() * Time.deltaTime;
                }
                break;
            case FishEncounterType.SequenceInput:
                UpdateSequencePhase();
                phaseMeter = (float)sequenceIndex / Mathf.Max(1, sequence.Length);
                break;
            case FishEncounterType.ReactionDodge:
                UpdateReactionPhase();
                phaseMeter = capture / maxCapture;
                break;
            case FishEncounterType.PrecisionStop:
                phaseMeter = Mathf.PingPong(Time.time * phase.difficulty, 1f);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    float accuracy = Mathf.Abs(0.5f - phaseMeter);
                    if (accuracy <= 0.12f + GetFocusBonus())
                    {
                        AddCapture(GetCapturePower(22f));
                    }
                    else
                    {
                        tension += 12f;
                    }
                }
                break;
            default:
                phaseMeter = Mathf.PingPong(Time.time * phase.difficulty, 1f);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (phaseMeter >= safeStart && phaseMeter <= safeEnd)
                    {
                        AddCapture(GetCapturePower(18f));
                    }
                    else
                    {
                        tension += 10f;
                    }
                }
                break;
        }

        tension = Mathf.Clamp(tension, 0f, maxTension);
    }

    private void UpdateSequencePhase()
    {
        if (sequenceIndex >= sequence.Length)
        {
            AddCapture(GetCapturePower(20f));
            sequenceIndex = 0;
            sequence = BuildSequence(Mathf.Clamp(2 + phaseIndex, 2, 6));
            return;
        }

        KeyCode expected = CharToKey(sequence[sequenceIndex]);
        foreach (KeyCode key in sequenceKeys)
        {
            if (!Input.GetKeyDown(key))
            {
                continue;
            }

            if (key == expected)
            {
                sequenceIndex++;
            }
            else
            {
                tension += Mathf.Max(6f, 12f - GetReflexBonus() * 20f);
            }
        }
    }

    private void UpdateReactionPhase()
    {
        foreach (KeyCode key in sequenceKeys)
        {
            if (!Input.GetKeyDown(key))
            {
                continue;
            }

            if (key == reactionKey)
            {
                AddCapture(GetCapturePower(16f));
                reactionKey = PickPromptKey();
            }
            else
            {
                tension += Mathf.Max(6f, 14f - GetReflexBonus() * 20f);
            }
        }
    }

    private void AdvancePhase()
    {
        if (phaseIndex >= activeBoss.phases.Count - 1)
        {
            FinishEncounter(true);
            return;
        }

        StartPhase(phaseIndex + 1);
    }

    private void FinishEncounter(bool caught)
    {
        encounterActive = false;
        encounterAvailable = false;

        BossEncounterResult result = BuildResult(caught);

        if (RunManager.Instance != null)
        {
            RunManager.Instance.CompleteBossEncounter(result);
        }
    }

    private BossEncounterResult BuildResult(bool caught)
    {
        int gold = caught ? GetModifiedGoldReward() : Mathf.RoundToInt(activeBoss.goldReward * 0.15f);
        int xp = caught ? GetModifiedXPReward() : Mathf.RoundToInt(activeBoss.xpReward * 0.25f);
        bool gearDropped = caught && Random.value < GetModifiedGearDropChance();
        string gearName = "";

        if (gearDropped && activeInteractor != null && activeInteractor.wizardGear != null)
        {
            GearItem item = activeInteractor.wizardGear.RollAndEquipRandomGear();
            gearName = item != null ? item.GetDisplayName() : "Random boss gear";
        }

        return new BossEncounterResult
        {
            caught = caught,
            bossName = activeBoss.bossName,
            bossFishItemName = caught ? "Boss Fish: " + activeBoss.bossName : "",
            goldEarned = gold,
            xpEarned = xp,
            gearDropped = gearDropped,
            gearDropName = gearName
        };
    }

    private BossFishData ChooseBoss()
    {
        if (bossPool == null || bossPool.Count == 0)
        {
            bossPool = new List<BossFishData> { new BossFishData() };
        }

        return bossPool[Random.Range(0, bossPool.Count)];
    }

    private BossPhaseData GetCurrentPhase()
    {
        if (activeBoss.phases == null || activeBoss.phases.Count == 0)
        {
            activeBoss.phases = new List<BossPhaseData> { new BossPhaseData() };
        }

        return activeBoss.phases[Mathf.Clamp(phaseIndex, 0, activeBoss.phases.Count - 1)];
    }

    private void ResetMeters()
    {
        tension = 0f;
        capture = 0f;
        phaseMeter = 0f;
        UpdateSliders();
    }

    private float GetPhaseCaptureTarget()
    {
        float target = 0f;
        for (int i = 0; i <= phaseIndex && i < activeBoss.phases.Count; i++)
        {
            target += activeBoss.phases[i].captureRequired;
        }

        return Mathf.Min(maxCapture, target);
    }

    private void AddCapture(float amount)
    {
        capture += amount;
        capture = Mathf.Clamp(capture, 0f, maxCapture);
        tension -= GetControlBonus() * 0.35f;
    }

    private float GetCapturePower(float baseAmount)
    {
        float powerBonus = GetTotalStat(WizardStatType.Power) * 0.03f;
        return baseAmount * (1f + powerBonus);
    }

    private float GetModifiedTensionIncrease(BossPhaseData phase)
    {
        float gearControl = GetGearAffix(GearAffixType.BossTensionControlPercent) / 100f;
        float control = GetTotalStat(WizardStatType.Control) * 0.015f;
        return phase.tensionIncreasePerSecond * Mathf.Clamp(1f - gearControl - control, 0.55f, 1.25f);
    }

    private float GetModifiedPhaseDuration(BossPhaseData phase)
    {
        return phase.duration + GetTotalStat(WizardStatType.Reflex) * 0.08f;
    }

    private float GetSafeWindowSize(BossPhaseData phase)
    {
        float focus = GetTotalStat(WizardStatType.Focus) * 0.006f;
        return Mathf.Clamp(0.28f - phase.difficulty * 0.025f + focus, 0.12f, 0.5f);
    }

    private float GetFocusBonus()
    {
        return GetTotalStat(WizardStatType.Focus) * 0.004f;
    }

    private float GetReflexBonus()
    {
        return GetTotalStat(WizardStatType.Reflex) * 0.01f;
    }

    private float GetControlBonus()
    {
        return GetTotalStat(WizardStatType.Control) * 0.1f;
    }

    private float GetControlMultiplier()
    {
        return Mathf.Clamp(1f - GetTotalStat(WizardStatType.Control) * 0.015f, 0.65f, 1f);
    }

    private int GetModifiedGoldReward()
    {
        float bonus = GetTotalStat(WizardStatType.Wisdom) * 0.01f;
        bonus += GetGearAffix(GearAffixType.SellValuePercent) / 100f;
        return Mathf.RoundToInt(activeBoss.goldReward * (1f + bonus));
    }

    private int GetModifiedXPReward()
    {
        float bonus = GetTotalStat(WizardStatType.Wisdom) * 0.015f;
        bonus += GetGearAffix(GearAffixType.FishXPPercent) / 100f;
        return Mathf.RoundToInt(activeBoss.xpReward * (1f + bonus));
    }

    private float GetModifiedGearDropChance()
    {
        float chance = activeBoss.gearDropChance + GetTotalStat(WizardStatType.Luck) * 0.01f;
        chance += GetGearAffix(GearAffixType.RareFishChancePercent) / 200f;
        return Mathf.Clamp01(chance);
    }

    private int GetTotalStat(WizardStatType statType)
    {
        if (activeInteractor == null)
        {
            return 0;
        }

        int total = activeInteractor.wizardStats != null ? activeInteractor.wizardStats.GetStat(statType) : 0;
        total += activeInteractor.wizardGear != null ? activeInteractor.wizardGear.GetStatBonus(statType) : 0;
        return total;
    }

    private float GetGearAffix(GearAffixType affixType)
    {
        if (activeInteractor == null || activeInteractor.wizardGear == null)
        {
            return 0f;
        }

        return activeInteractor.wizardGear.GetAffixValue(affixType);
    }

    private void UpdateBossHud(BossPhaseData phase)
    {
        UpdateSliders();

        if (RunManager.Instance == null || RunManager.Instance.prototypeHud == null)
        {
            return;
        }

        string prompt = GetPhasePrompt(phase);
        string bonusText = "Bonuses: "
            + "Control stabilized the boss line. "
            + "Power increased capture progress. "
            + "Luck improves reward rolls. "
            + "Wisdom improves boss XP/gold.";

        RunManager.Instance.prototypeHud.SetBossStatus(
            activeBoss.bossName + " (" + activeBoss.bossRarity + ", Lv " + activeBoss.bossLevel + ")\n"
            + "Phase " + (phaseIndex + 1) + "/" + activeBoss.phases.Count + ": " + phase.phaseName + "\n"
            + phase.instructions + "\n"
            + prompt + "\n"
            + "Tension: " + Mathf.CeilToInt(tension) + " / " + Mathf.CeilToInt(maxTension) + "\n"
            + "Capture: " + Mathf.CeilToInt(capture) + " / " + Mathf.CeilToInt(maxCapture) + "\n"
            + "Time: " + Mathf.CeilToInt(phaseTimer) + "s\n"
            + bonusText + "\n"
            + activeBoss.flavorText);
    }

    private void UpdateSliders()
    {
        if (tensionSlider != null)
        {
            tensionSlider.maxValue = maxTension;
            tensionSlider.value = tension;
        }

        if (captureSlider != null)
        {
            captureSlider.maxValue = maxCapture;
            captureSlider.value = capture;
        }
    }

    private string GetPhasePrompt(BossPhaseData phase)
    {
        switch (phase.encounterType)
        {
            case FishEncounterType.RapidTap:
                return "Tap Space rapidly.";
            case FishEncounterType.HoldBalance:
                return "Hold/release Space. Safe zone: " + safeStart.ToString("0.00") + " - " + safeEnd.ToString("0.00") + " Meter: " + phaseMeter.ToString("0.00");
            case FishEncounterType.SequenceInput:
                return "Sequence: " + sequence + " Next: " + CharToKey(sequence[Mathf.Clamp(sequenceIndex, 0, sequence.Length - 1)]);
            case FishEncounterType.ReactionDodge:
                return "Press: " + reactionKey;
            case FishEncounterType.PrecisionStop:
                return "Press Space near center. Meter: " + phaseMeter.ToString("0.00");
            default:
                return "Press Space in safe zone: " + safeStart.ToString("0.00") + " - " + safeEnd.ToString("0.00") + " Meter: " + phaseMeter.ToString("0.00");
        }
    }

    private string BuildSequence(int length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += KeyToChar(sequenceKeys[Random.Range(0, sequenceKeys.Length)]);
        }

        return result;
    }

    private KeyCode PickPromptKey()
    {
        return sequenceKeys[Random.Range(0, sequenceKeys.Length)];
    }

    private KeyCode CharToKey(char value)
    {
        switch (value)
        {
            case 'W':
                return KeyCode.W;
            case 'A':
                return KeyCode.A;
            case 'S':
                return KeyCode.S;
            default:
                return KeyCode.D;
        }
    }

    private char KeyToChar(KeyCode key)
    {
        if (key == KeyCode.W) return 'W';
        if (key == KeyCode.A) return 'A';
        if (key == KeyCode.S) return 'S';
        return 'D';
    }
}
