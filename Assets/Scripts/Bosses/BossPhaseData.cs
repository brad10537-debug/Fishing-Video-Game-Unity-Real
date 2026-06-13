using UnityEngine;

[System.Serializable]
public class BossPhaseData
{
    public string phaseName = "Phase";
    public FishEncounterType encounterType = FishEncounterType.RapidTap;
    [Range(0.5f, 5f)] public float difficulty = 1f;
    [Min(1f)] public float duration = 8f;
    [Min(1f)] public float captureRequired = 35f;
    [Min(0f)] public float tensionIncreasePerSecond = 5f;
    [TextArea] public string instructions = "Follow the prompt to fill the capture meter.";
}
