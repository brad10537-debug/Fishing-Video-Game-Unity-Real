using UnityEngine;

// Player-owned wizard stats. These numbers are intentionally simple so minigame
// systems can read them without owning progression or UI.
public class WizardStats : MonoBehaviour
{
    [Header("Stats")]
    public int focus = 0;
    public int reflex = 0;
    public int luck = 0;
    public int control = 0;
    public int power = 0;
    public int wisdom = 0;

    [Header("Unspent Points")]
    public int statPoints = 0;

    public event System.Action StatsChanged;

    private void Update()
    {
        if (statPoints <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F1)) SpendPoint("Focus");
        if (Input.GetKeyDown(KeyCode.F2)) SpendPoint("Reflex");
        if (Input.GetKeyDown(KeyCode.F3)) SpendPoint("Luck");
        if (Input.GetKeyDown(KeyCode.F4)) SpendPoint("Control");
        if (Input.GetKeyDown(KeyCode.F5)) SpendPoint("Power");
        if (Input.GetKeyDown(KeyCode.F6)) SpendPoint("Wisdom");
    }

    public void AddStatPoints(int amount)
    {
        statPoints += Mathf.Max(0, amount);
        Debug.Log("Stat points available: " + statPoints);
        StatsChanged?.Invoke();
    }

    public bool SpendPoint(string statName)
    {
        if (statPoints <= 0)
        {
            Debug.Log("No stat points available.");
            return false;
        }

        switch (statName)
        {
            case "Focus":
                focus++;
                break;
            case "Reflex":
                reflex++;
                break;
            case "Luck":
                luck++;
                break;
            case "Control":
                control++;
                break;
            case "Power":
                power++;
                break;
            case "Wisdom":
                wisdom++;
                break;
            default:
                Debug.Log("Unknown stat: " + statName);
                return false;
        }

        statPoints--;
        Debug.Log("Spent 1 point on " + statName + ". Points left: " + statPoints);
        StatsChanged?.Invoke();
        return true;
    }

    public int GetStat(WizardStatType statType)
    {
        switch (statType)
        {
            case WizardStatType.Focus:
                return focus;
            case WizardStatType.Reflex:
                return reflex;
            case WizardStatType.Luck:
                return luck;
            case WizardStatType.Control:
                return control;
            case WizardStatType.Power:
                return power;
            case WizardStatType.Wisdom:
                return wisdom;
            default:
                return 0;
        }
    }
}
