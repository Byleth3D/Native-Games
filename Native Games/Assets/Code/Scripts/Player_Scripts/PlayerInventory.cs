using UnityEngine;

public class PlayerInventory : InteractorSystem
{
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
            interactionTrigger.OnTriggerInteract();
            interactionTrigger.OnTriggerInteractCanceled();
        }
        else
        {
            DeliverableObject deliverableObject = interactable as DeliverableObject;

            if (deliverableObject.requestedAmountToDeliver == itemCount)
            {
                itemCount = 0;
                interactionTrigger.OnTriggerInteract();
            }
            else
            {
                interactionTrigger.OnTriggerInteractCanceled();
            }
        }
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
