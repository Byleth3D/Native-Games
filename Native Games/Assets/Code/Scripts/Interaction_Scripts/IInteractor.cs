
public interface IInteractor
{
    InteractorType Interactor { get; }
    InteractableType CompatibleInteractions { get; }
    void OnInteractionEnter(IInteractable interactable);
    void OnInteractionExit();
}

public enum InteractorType
{
    System,
    Agent
}