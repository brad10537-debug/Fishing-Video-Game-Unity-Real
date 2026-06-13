using UnityEngine;

// Simple seat for benches, chairs, logs, or campfires.
// The player snaps to the sit point and movement pauses until E or movement input exits.
public class SitPointInteractable : MonoBehaviour, IInteractable
{
    public string displayName = "Cozy Bench";
    public Transform sitPoint;
    public string sittingMessage = "Sitting";

    public string GetInteractPrompt()
    {
        return "Press E to sit at " + displayName;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (interactor == null)
        {
            return;
        }

        FirstPersonPrototypeController movementController = interactor.movementController;
        if (movementController == null)
        {
            movementController = interactor.GetComponent<FirstPersonPrototypeController>();
        }

        if (movementController == null)
        {
            interactor.ShowStatus("This needs a FirstPersonPrototypeController on the player.");
            return;
        }

        movementController.EnterSitting(sitPoint != null ? sitPoint : transform);
        interactor.ShowStatus(sittingMessage + ". Press E or move to stand up.");
    }
}
