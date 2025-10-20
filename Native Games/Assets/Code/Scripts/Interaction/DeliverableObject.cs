using UnityEngine;
using UnityEngine.Events;

public class DeliverableObject : MonoBehaviour, IInteractable
{
    [SerializeField] public InventoryItem inventoryItem;
    [SerializeField] public int requestedItemAmount = 3;
    [Space]
    [SerializeField] private UnityEvent OnDeliver;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Deliver;
    public bool Delivered { get; private set; }

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        GameplayUIManager.Instance.popupManager.DisableActivePopup();
        GameplayUIManager.Instance.popupManager.EnablePopup("Interact");
        InteractionTrigger = interactionTrigger;
    }

    public void Interaction()
    {
        if (Delivered)
        {
            OnDeliver?.Invoke();
            return;
        }

        GameplayUIManager.Instance.popupManager.DisableActivePopup();
        GameplayUIManager.Instance.popupManager.EnablePopup("Interact");

        if (InventoryManager.Instance.Deliver(inventoryItem, requestedItemAmount))
        {
            OnDeliver?.Invoke();
            Delivered = true;
        }
        else
        {
            InteractionCancel();
        }
    }

    public void InteractionCancel()
    {
        GameplayUIManager.Instance.popupManager.DisableActivePopup();
        GameplayUIManager.Instance.popupManager.EnablePopup($"{inventoryItem.name}Fail");
        InteractionTrigger.TriggerInteractCancel(false);
    }

    public void InteractionExit()
    {
        GameplayUIManager.Instance.popupManager.DisableActivePopup();
    }
}
