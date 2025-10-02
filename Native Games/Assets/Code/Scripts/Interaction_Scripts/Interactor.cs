using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] protected InteractorType interactorDefinition;
    [SerializeField] protected InteractableType[] interactableFilter;

    public InteractorType GetInteractorDefinition() => interactorDefinition;
    public InteractableType[] GetInteractableFilter() => interactableFilter;
    public bool HasInteractableInFilter(InteractableType targetInteractableType)
    {
        foreach (InteractableType interactableType in interactableFilter)
        {
            if (interactableType == targetInteractableType)
            {
                return true;
            }
        }

        return false;
    }

    public abstract void OnInteractionEnter(InteractionTrigger interactionTrigger);
    public abstract void OnInteraction();
    public abstract void OnInteractionCanceled();
    public abstract void OnInteractionExit();
}

public enum InteractorType
{
    System,
    Agent
}