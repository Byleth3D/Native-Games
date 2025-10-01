using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] protected InteractableType interactableDefinition;
    [SerializeField] protected InteractorType interactorFilter;

    public InteractableType GetInteractableDefinition() => interactableDefinition;
    public InteractorType GetInteractorFilter() => interactorFilter;

    public abstract void OnInteractionEnter(InteractionTrigger interactionTrigger);
    public abstract void OnInteraction();
    public abstract void OnInteractionCanceled();
    public abstract void OnInteractionExit();
}

public enum InteractableType
{
    ItemCollect,
    ItemDeliver,
    Prompt,
    ObjectPush,
    None
}