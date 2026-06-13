using UnityEngine;

// Text-based NPC interaction using the existing IInteractable system.
// CharacterIdentity owns the name/role/faction; this script owns the line spoken to the player.
public class NPCInteractable : MonoBehaviour, IInteractable
{
    public CharacterIdentity identity;
    public VillageAreaType hangoutArea = VillageAreaType.MainPond;
    [TextArea] public string dialogueLine = "Lovely day for impossible fishing.";
    public bool showAreaInDialogue = true;

    private void Awake()
    {
        if (identity == null)
        {
            identity = GetComponent<CharacterIdentity>();
        }
    }

    public string GetInteractPrompt()
    {
        string npcName = identity != null ? identity.displayName : gameObject.name;
        return "Press E to talk to " + npcName;
    }

    public void Interact(PlayerInteractor interactor)
    {
        string npcName = identity != null ? identity.displayName : gameObject.name;
        string role = identity != null ? identity.titleOrRole : "Villager";
        string message = npcName + " - " + role + "\n" + dialogueLine;

        if (showAreaInDialogue)
        {
            message += "\nArea: " + hangoutArea;
        }

        if (interactor != null)
        {
            interactor.ShowStatus(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
