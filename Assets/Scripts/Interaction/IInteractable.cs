public interface IInteractable
{
    string GetInteractPrompt();
    void Interact(PlayerInteractor interactor);
}
