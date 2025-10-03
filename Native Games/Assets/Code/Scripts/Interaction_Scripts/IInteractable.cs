public interface IInteractable
{
    InteractionTrigger InteractionTrigger { get; }
    InteractionType InteractionType { get; }

    void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController);
    void Interaction();
    void InteractionCancel();
    void InteractionExit();
}

public enum InteractionType
{
    Push,
    Collect,
    Deliver,
    Prompt
}