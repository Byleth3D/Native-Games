using UnityEngine;

public interface IInteractable
{
    InteractableType Interactable { get; }
    void OnInteractionEnter(IInteractorAgent interactor);
    void OnInteract();
    void OnInteractCancel();
    void OnInteractionExit();
}

public enum InteractableType
{
    ItemCollect,
    Prompt,
    ObjectPush,
    None
}