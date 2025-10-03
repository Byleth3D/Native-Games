using UnityEngine;
using UnityEngine.Events;

public class DeliverableObject : MonoBehaviour, IInteractable
{
    [SerializeField] public int requestedItemAmount = 3;
    [SerializeField] private UnityEvent OnInteraction;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Deliver;

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        InteractionTrigger = interactionTrigger;
        UIManager.Instance.EnableInteractPopUp();
    }

    public void Interaction()
    {
        UIManager.Instance.DisableInteractPopUp();

        if (InventoryManager.Instance.ItemCount == requestedItemAmount)
        {
            InventoryManager.Instance.Deliver();
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
        UIManager.Instance.EnableInteractFailedPopUp();
        InteractionTrigger.TriggerInteractCancel(false);
    }

    public void InteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
        UIManager.Instance.DisableInteractFailedPopUp();
    }
}
