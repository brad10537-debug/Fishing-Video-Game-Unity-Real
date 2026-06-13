using UnityEngine;

// Starts a roguelite fishing run from the village lobby.
public class FishingRunPortalInteractable : MonoBehaviour, IInteractable
{
    public string portalName = "Moonlit Fishing Dock";
    public RunManager runManager;

    private void Awake()
    {
        if (runManager == null)
        {
            runManager = FindAnyObjectByType<RunManager>();
        }
    }

    public string GetInteractPrompt()
    {
        if (runManager != null && runManager.CurrentState != RunState.Lobby && runManager.CurrentState != RunState.RunComplete)
        {
            return "A run is already active.";
        }

        return "Press E to start a fishing run at " + portalName + ".";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (runManager == null)
        {
            if (interactor != null)
            {
                interactor.ShowStatus("No RunManager found in the scene.");
            }
            return;
        }

        runManager.StartRun();

        if (interactor != null)
        {
            interactor.ShowStatus("The magical fishing run begins!");
        }
    }
}
