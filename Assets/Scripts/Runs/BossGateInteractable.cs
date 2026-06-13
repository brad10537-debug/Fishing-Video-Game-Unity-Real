using UnityEngine;

// Decorative lobby gate for the future boss entrance.
// For now it reports whether the run loop says a boss encounter is ready.
public class BossGateInteractable : MonoBehaviour, IInteractable
{
    public string gateName = "Boss Gate";
    [TextArea] public string lockedMessage = "The arch hums quietly. Finish the fishing rounds before challenging a boss fish.";
    [TextArea] public string readyMessage = "The boss gate crackles with pond magic. The boss encounter is ready.";

    public string GetInteractPrompt()
    {
        return "Press E to inspect " + gateName;
    }

    public void Interact(PlayerInteractor interactor)
    {
        bool bossReady = RunManager.Instance != null && RunManager.Instance.CurrentState == RunState.BossEncounter;
        string message = bossReady ? readyMessage : lockedMessage;

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
