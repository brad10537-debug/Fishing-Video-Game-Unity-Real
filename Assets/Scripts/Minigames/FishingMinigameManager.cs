using System;
using System.Collections.Generic;
using UnityEngine;

// Runs simple text-based fishing quick-time minigames and returns a catch quality.
// It does not add fish to inventory directly; FishingSpotInteractable handles that result.
public class FishingMinigameManager : MonoBehaviour
{
    public static FishingMinigameManager Instance { get; private set; }

    [Header("Prototype Timing")]
    public float baseDuration = 6f;
    public KeyCode actionKey = KeyCode.Space;
    public bool logMinigameUpdates = false;

    private FishData activeFish;
    private FishSpeciesData activeSpecies;
    private PlayerInteractor activeInteractor;
    private Action<FishData, CatchQuality> onComplete;
    private bool active;
    private float timer;
    private float meter;
    private float safeStart;
    private float safeEnd;
    private int successScore;
    private int failures;
    private int effectiveRequiredSuccessScore;
    private int effectiveFailureTolerance;
    private int sequenceIndex;
    private string sequence;
    private KeyCode reactionKey;
    private FishEncounterType activeEncounterType;
    private float difficulty;
    private string lastFailureReason;
    private float controlMultiplier = 1f;
    private int rapidTapGain = 1;
    private float perfectWindowBonus;
    private string bonusSummary;

    private readonly KeyCode[] sequenceKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private readonly List<FishBehaviorType> seenBehaviorTypes = new List<FishBehaviorType>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Finish(CatchQuality.Failed, "Cancelled.");
            return;
        }

        switch (activeEncounterType)
        {
            case FishEncounterType.RapidTap:
                UpdateRapidTap();
                break;
            case FishEncounterType.HoldBalance:
                UpdateHoldBalance();
                break;
            case FishEncounterType.SequenceInput:
                UpdateSequenceInput();
                break;
            case FishEncounterType.ReactionDodge:
                UpdateReactionDodge();
                break;
            case FishEncounterType.PrecisionStop:
                UpdatePrecisionStop();
                break;
            default:
                UpdateTimingWindow();
                break;
        }

        if (timer <= 0f)
        {
            Finish(CalculateQuality(), "Tension dropped before the fish was secured.");
        }
    }

    public string GetLastFailureReason()
    {
        return lastFailureReason;
    }

    public void StartMinigame(FishData fish, FishSpeciesData species, float scaledDifficulty, PlayerInteractor interactor, Action<FishData, CatchQuality> completed)
    {
        activeFish = fish;
        activeSpecies = species;
        activeEncounterType = ChooseEncounterTypeForBehavior(species);
        difficulty = Mathf.Max(0.5f, scaledDifficulty);
        activeInteractor = interactor;
        onComplete = completed;
        active = true;
        timer = Mathf.Max(2f, baseDuration + 2f - difficulty);
        meter = 0f;
        successScore = 0;
        failures = 0;
        lastFailureReason = "";
        effectiveRequiredSuccessScore = species.requiredSuccessScore;
        effectiveFailureTolerance = species.failureTolerance;
        sequenceIndex = 0;
        ApplyWizardBonuses();
        sequence = BuildSequence(Mathf.Clamp(effectiveRequiredSuccessScore, 2, 6));
        reactionKey = PickPromptKey();

        float safeSize = Mathf.Clamp(0.45f - difficulty * 0.03f, 0.14f, 0.45f);
        safeSize += GetTotalStat(WizardStatType.Focus) * 0.006f;
        safeSize += perfectWindowBonus;
        safeSize = Mathf.Clamp(safeSize, 0.14f, 0.6f);
        safeStart = UnityEngine.Random.Range(0.1f, 0.9f - safeSize);
        safeEnd = safeStart + safeSize;

        string firstTimeHint = GetFirstTimeBehaviorHint(species.behaviorType);
        ShowMinigame("Hooked: " + fish.GetDisplayName(), GetInstructions(), timer, meter, GetCurrentPromptText(), successScore, effectiveRequiredSuccessScore, firstTimeHint + bonusSummary);
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayFishHook();
        }

        Debug.Log("Fishing minigame started: " + fish.GetDisplayName() + ". Behavior: " + fish.GetBehaviorDisplayName() + ". " + GetInstructions());
    }

    private void UpdateTimingWindow()
    {
        meter = Mathf.PingPong(Time.time * activeSpecies.speedModifier * difficulty, 1f);

        if (Input.GetKeyDown(actionKey))
        {
            Finish(meter >= safeStart && meter <= safeEnd ? QualityFromAccuracy(GetSafeZoneAccuracy()) : CatchQuality.Failed, "Missed timing.");
        }

        ShowMinigame("Timing Window", GetInstructions(), timer, meter, "Press Space when the moving bar is inside the safe zone.", successScore, effectiveRequiredSuccessScore, "Safe zone: " + safeStart.ToString("0.00") + " - " + safeEnd.ToString("0.00") + "\n" + bonusSummary);
    }

    private void UpdateRapidTap()
    {
        if (Input.GetKeyDown(actionKey))
        {
            successScore += rapidTapGain;
        }

        meter = Mathf.Clamp01((float)successScore / effectiveRequiredSuccessScore);
        ShowMinigame("Rapid Tap", GetInstructions(), timer, meter, "Tap Space repeatedly until the meter fills.", successScore, effectiveRequiredSuccessScore, bonusSummary);

        if (successScore >= effectiveRequiredSuccessScore)
        {
            Finish(QualityFromProgress(meter, timer));
        }
    }

    private void UpdateHoldBalance()
    {
        meter += (Input.GetKey(actionKey) ? 1f : -1f) * Time.deltaTime * activeSpecies.speedModifier * difficulty * 0.55f * controlMultiplier;
        meter = Mathf.Clamp01(meter);

        if (meter >= safeStart && meter <= safeEnd)
        {
            successScore++;
        }

        ShowMinigame("Hold Balance", GetInstructions(), timer, meter, "Hold Space to raise the meter. Release Space to lower it.", successScore, effectiveRequiredSuccessScore, "Keep the meter inside the safe zone.\n" + bonusSummary);

        if (successScore >= effectiveRequiredSuccessScore * 25)
        {
            Finish(CatchQuality.Great);
        }
    }

    private void UpdateSequenceInput()
    {
        if (sequenceIndex >= sequence.Length)
        {
            Finish(QualityFromMistakes());
            return;
        }

        KeyCode expected = CharToKey(sequence[sequenceIndex]);
        foreach (KeyCode key in sequenceKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == expected)
                {
                    sequenceIndex++;
                    successScore++;
                }
                else
                {
                    failures++;
                    lastFailureReason = "Wrong input.";
                }
            }
        }

        meter = Mathf.Clamp01((float)sequenceIndex / sequence.Length);
        ShowMinigame("Sequence Input", GetInstructions(), timer, meter, "Press the next key: " + expected, sequenceIndex, sequence.Length, "Full sequence: " + sequence + "\nMistakes: " + failures + " / " + effectiveFailureTolerance + "\n" + bonusSummary);
    }

    private void UpdateReactionDodge()
    {
        foreach (KeyCode key in sequenceKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == reactionKey)
                {
                    successScore++;
                    reactionKey = PickPromptKey();
                }
                else
                {
                    failures++;
                    lastFailureReason = "Wrong input.";
                }
            }
        }

        meter = Mathf.Clamp01((float)successScore / effectiveRequiredSuccessScore);
        ShowMinigame("Reaction Dodge", GetInstructions(), timer, meter, "Quick! Press " + reactionKey, successScore, effectiveRequiredSuccessScore, "Mistakes: " + failures + " / " + effectiveFailureTolerance + "\n" + bonusSummary);

        if (successScore >= effectiveRequiredSuccessScore)
        {
            Finish(QualityFromMistakes());
        }
    }

    private void UpdatePrecisionStop()
    {
        meter = Mathf.PingPong(Time.time * activeSpecies.speedModifier * difficulty * 1.25f, 1f);

        if (Input.GetKeyDown(actionKey))
        {
            Finish(QualityFromAccuracy(Mathf.Abs(0.5f - meter) * 2f));
        }

        ShowMinigame("Precision Stop", GetInstructions(), timer, meter, "Press Space when the moving bar is closest to the center.", successScore, effectiveRequiredSuccessScore, bonusSummary);
    }

    private void Finish(CatchQuality quality, string failureReason = "")
    {
        if (!active)
        {
            return;
        }

        active = false;
        if (quality == CatchQuality.Failed)
        {
            lastFailureReason = string.IsNullOrEmpty(failureReason) ? lastFailureReason : failureReason;
        }
        else
        {
            lastFailureReason = "";
        }
        string resultMessage = quality == CatchQuality.Failed
            ? "The fish got away! " + lastFailureReason
            : "Catch Quality: " + quality;
        ShowMinigame("Result", resultMessage, 0f, 0f, "", successScore, effectiveRequiredSuccessScore, bonusSummary);
        if (GameAudioFeedback.Instance != null)
        {
            if (quality == CatchQuality.Failed)
            {
                GameAudioFeedback.Instance.PlayMinigameFailure();
            }
            else
            {
                GameAudioFeedback.Instance.PlayMinigameSuccess();
            }
        }

        Debug.Log(resultMessage);
        onComplete?.Invoke(activeFish, quality);
    }

    private CatchQuality CalculateQuality()
    {
        if (failures > effectiveFailureTolerance)
        {
            lastFailureReason = "Too many missed inputs.";
            return CatchQuality.Failed;
        }

        float progress = activeEncounterType == FishEncounterType.SequenceInput
            ? (float)sequenceIndex / Mathf.Max(1, sequence.Length)
            : (float)successScore / Mathf.Max(1, effectiveRequiredSuccessScore);

        return QualityFromProgress(progress, timer);
    }

    private CatchQuality QualityFromProgress(float progress, float remainingTime)
    {
        if (progress < 0.5f)
        {
            return CatchQuality.Failed;
        }

        if (progress >= 1f && remainingTime > baseDuration * 0.45f)
        {
            return CatchQuality.Perfect;
        }

        if (progress >= 1f)
        {
            return CatchQuality.Great;
        }

        return progress >= 0.75f ? CatchQuality.Good : CatchQuality.Poor;
    }

    private CatchQuality QualityFromMistakes()
    {
        if (failures > effectiveFailureTolerance)
        {
            lastFailureReason = "Too many missed inputs.";
            return CatchQuality.Failed;
        }

        if (failures == 0 && timer > baseDuration * 0.25f)
        {
            return CatchQuality.Perfect;
        }

        if (failures == 0)
        {
            return CatchQuality.Great;
        }

        return failures == 1 ? CatchQuality.Good : CatchQuality.Poor;
    }

    private CatchQuality QualityFromAccuracy(float accuracy)
    {
        if (accuracy <= 0.08f)
        {
            return CatchQuality.Perfect;
        }

        if (accuracy <= 0.18f)
        {
            return CatchQuality.Great;
        }

        if (accuracy <= 0.35f)
        {
            return CatchQuality.Good;
        }

        return CatchQuality.Poor;
    }

    private float GetSafeZoneAccuracy()
    {
        float center = (safeStart + safeEnd) * 0.5f;
        return Mathf.Abs(center - meter);
    }

    private string GetInstructions()
    {
        switch (activeEncounterType)
        {
            case FishEncounterType.RapidTap:
                return activeSpecies.speciesName + " is darting. Tap Space quickly to fill the catch meter.";
            case FishEncounterType.HoldBalance:
                return activeSpecies.speciesName + " is pulling. Hold and release Space to balance tension.";
            case FishEncounterType.SequenceInput:
                return activeSpecies.speciesName + " flashes left and right. Press the prompted WASD keys in order.";
            case FishEncounterType.ReactionDodge:
                return activeSpecies.speciesName + " fights dirty. Watch for changing prompts and press the correct WASD key.";
            case FishEncounterType.PrecisionStop:
                return activeSpecies.speciesName + " is shimmering. Press Space when the marker is close to the center.";
            default:
                return activeSpecies.speciesName + " is nibbling. Press Space in the green zone.";
        }
    }

    private void ApplyWizardBonuses()
    {
        int focus = GetTotalStat(WizardStatType.Focus);
        int reflex = GetTotalStat(WizardStatType.Reflex);
        int control = GetTotalStat(WizardStatType.Control);
        int power = GetTotalStat(WizardStatType.Power);

        float rapidTapPercent = GetGearAffix(GearAffixType.RapidTapPowerPercent);
        float perfectWindowPercent = GetGearAffix(GearAffixType.PerfectCatchWindowPercent);
        int extraSequenceMistakes = Mathf.RoundToInt(GetGearAffix(GearAffixType.SequenceMistakeAllowance));

        perfectWindowBonus = perfectWindowPercent / 100f;
        controlMultiplier = Mathf.Clamp(1f - control * 0.015f, 0.65f, 1f);
        rapidTapGain = Mathf.Max(1, Mathf.RoundToInt(1f + power * 0.05f + rapidTapPercent / 100f));
        effectiveFailureTolerance += extraSequenceMistakes;

        if (activeEncounterType == FishEncounterType.ReactionDodge || activeEncounterType == FishEncounterType.SequenceInput)
        {
            timer += reflex * 0.05f;
        }

        bonusSummary = "Bonuses: "
            + "Focus +" + focus + " widened timing. "
            + "Reflex +" + reflex + " added reaction time. "
            + "Control +" + control + " stabilized balance. "
            + "Power +" + power + " improved tap strength.";

        if (perfectWindowPercent > 0f)
        {
            bonusSummary += " Gear +" + perfectWindowPercent.ToString("0") + "% perfect window.";
        }

        if (rapidTapPercent > 0f)
        {
            bonusSummary += " Gear +" + rapidTapPercent.ToString("0") + "% rapid tap power.";
        }

        if (extraSequenceMistakes > 0)
        {
            bonusSummary += " Gear +" + extraSequenceMistakes + " sequence mistake allowance.";
        }
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

    private string GetCurrentPromptText()
    {
        switch (activeEncounterType)
        {
            case FishEncounterType.SequenceInput:
                return "Sequence: " + sequence;
            case FishEncounterType.ReactionDodge:
                return "Press: " + reactionKey;
            case FishEncounterType.HoldBalance:
                return "Hold/release Space";
            case FishEncounterType.RapidTap:
                return "Tap Space";
            case FishEncounterType.PrecisionStop:
                return "Stop near center";
            default:
                return "Press Space in the safe window";
        }
    }

    private void ShowMinigame(string title, string instructions, float timeLeft, float progress, string prompt, int currentScore, int targetScore, string extra)
    {
        if (activeInteractor != null && activeInteractor.prototypeHud != null)
        {
            activeInteractor.prototypeHud.SetFishingMinigame(title, instructions, timeLeft, progress, prompt, currentScore, targetScore, extra);
        }

        if (logMinigameUpdates)
        {
            Debug.Log(title + " | " + instructions + " | " + prompt);
        }
    }

    private FishEncounterType ChooseEncounterTypeForBehavior(FishSpeciesData species)
    {
        if (species == null)
        {
            return FishEncounterType.TimingWindow;
        }

        switch (species.behaviorType)
        {
            case FishBehaviorType.RapidTap:
                return FishEncounterType.RapidTap;
            case FishBehaviorType.HoldBalance:
                return FishEncounterType.HoldBalance;
            case FishBehaviorType.ReactionSequence:
                return FishEncounterType.SequenceInput;
            case FishBehaviorType.Erratic:
                FishEncounterType[] erraticTypes =
                {
                    FishEncounterType.TimingWindow,
                    FishEncounterType.RapidTap,
                    FishEncounterType.HoldBalance,
                    FishEncounterType.ReactionDodge,
                    FishEncounterType.SequenceInput
                };
                return erraticTypes[UnityEngine.Random.Range(0, erraticTypes.Length)];
            default:
                return FishEncounterType.TimingWindow;
        }
    }

    private string GetFirstTimeBehaviorHint(FishBehaviorType behaviorType)
    {
        if (seenBehaviorTypes.Contains(behaviorType))
        {
            return "";
        }

        seenBehaviorTypes.Add(behaviorType);

        switch (behaviorType)
        {
            case FishBehaviorType.RapidTap:
                return "New behavior: Rapid Tap. Tap Space repeatedly until the meter fills.\n";
            case FishBehaviorType.HoldBalance:
                return "New behavior: Hold Balance. Hold Space to raise tension and release to lower it.\n";
            case FishBehaviorType.ReactionSequence:
                return "New behavior: Reaction Sequence. Press the prompted WASD keys.\n";
            case FishBehaviorType.Erratic:
                return "New behavior: Erratic. Watch carefully because this fish changes tactics.\n";
            default:
                return "New behavior: Timing. Press Space when the marker is in the safe zone.\n";
        }
    }

    private string BuildSequence(int length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += KeyToChar(sequenceKeys[UnityEngine.Random.Range(0, sequenceKeys.Length)]);
        }

        return result;
    }

    private KeyCode PickPromptKey()
    {
        return sequenceKeys[UnityEngine.Random.Range(0, sequenceKeys.Length)];
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
