using UnityEngine;
using UnityEngine.Events;

public class DeliverableObject : MonoBehaviour, IInteractable
{
    [SerializeField] public InventoryItem inventoryItem;
    [SerializeField] public int requestedItemAmount = 3;
    [SerializeField] private UnityEvent OnInteraction;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Deliver;

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        InteractionTrigger = interactionTrigger;
        GameplayUIManager.Instance.EnableInteractPopUp();
    }

    public void Interaction()
    {
        GameplayUIManager.Instance.DisableInteractPopUp();

        if (InventoryManager.Instance.Deliver(inventoryItem, requestedItemAmount)) 
        {
            OnInteraction?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            InteractionCancel();
        }
    }

    public void InteractionCancel()
    {
        GameplayUIManager.Instance.EnableInteractFailedPopUp();
        InteractionTrigger.TriggerInteractCancel(false);
    }

    public void InteractionExit()
    {
        GameplayUIManager.Instance.DisableInteractPopUp();
        GameplayUIManager.Instance.DisableInteractFailedPopUp();
    }
}
