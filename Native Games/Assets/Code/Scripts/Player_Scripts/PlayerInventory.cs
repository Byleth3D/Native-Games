using UnityEngine;

public class PlayerInventory : Interactor
{
    private int itemCount = 0;
    private InteractionTrigger interactionTrigger;
    private Interactable interactable;

    private void Update()
    {
        if (interactable && InputManager.Instance.InteractPressed)
        {
            OnInteraction();
        }
    }

    public override void OnInteraction()
    {
        if (interactable.GetInteractableDefinition() == InteractableType.ItemCollect)
        {
            itemCount++;
        }
        else if (interactable.GetInteractableDefinition() == InteractableType.ItemDeliver)
        {
            //itemCount--;
            //no-op
        }

        interactionTrigger.OnTriggerInteract();
        interactionTrigger.OnTriggerInteractCanceled();
    }

    public override void OnInteractionCanceled() { }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        this.interactionTrigger = interactionTrigger;
        interactable = interactionTrigger.Interactable;
    }

    public override void OnInteractionExit()
    {
        interactionTrigger = null;
        interactable = null;
    }
}
