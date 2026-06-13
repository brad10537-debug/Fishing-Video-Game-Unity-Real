[System.Serializable]
public class GearAffix
{
    public GearAffixType affixType;
    public float value;

    public string GetDescription()
    {
        switch (affixType)
        {
            case GearAffixType.Focus:
            case GearAffixType.Reflex:
            case GearAffixType.Luck:
            case GearAffixType.Control:
            case GearAffixType.Power:
            case GearAffixType.Wisdom:
                return "+" + value.ToString("0") + " " + affixType;
            case GearAffixType.SequenceMistakeAllowance:
                return "+" + value.ToString("0") + " sequence mistake allowed";
            default:
                return "+" + value.ToString("0") + "% " + affixType;
        }
    }
}
