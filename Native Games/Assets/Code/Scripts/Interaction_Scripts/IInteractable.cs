using UnityEngine;

public interface IInteractable
{
    InteractionType Interaction { get; }
    void OnInteractionEnter(IInteractorAgent interactor);
    void OnInteract();
    void OnInteractCancel();
    void OnInteractionExit();
}

public enum InteractionType
{
    ItemCollect,
    Prompt,
    ObjectPush,
    None
}