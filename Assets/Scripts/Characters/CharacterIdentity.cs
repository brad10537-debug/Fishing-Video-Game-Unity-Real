using UnityEngine;

// Describes who a character is. This is display/data only, so later NPC,
// combat, or multiplayer systems can read it without owning identity text.
public class CharacterIdentity : MonoBehaviour
{
    public string displayName = "Young Pond Wizard";
    public CharacterType characterType = CharacterType.Player;
    public FactionTeam faction = FactionTeam.Player;
    public string titleOrRole = "Apprentice Angler";
    [TextArea] public string flavorText = "A funky wizard ready to fish the impossible.";

    public string GetDisplayText()
    {
        string title = string.IsNullOrEmpty(titleOrRole) ? characterType.ToString() : titleOrRole;
        return displayName + "\n" + title + " | " + faction;
    }

    public string GetFullDescription()
    {
        return GetDisplayText() + "\n" + flavorText;
    }
}
