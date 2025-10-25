using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeliverableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Deliver> delivers;
    [SerializeField] private string deliverFailPopupName;
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

        if (InventoryManager.Instance.Deliver(delivers))
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
        GameplayUIManager.Instance.popupManager.EnablePopup($"{deliverFailPopupName}");
        InteractionTrigger.TriggerInteractCancel(false);
    }

    public void InteractionExit()
    {
        GameplayUIManager.Instance.popupManager.DisableActivePopup();
    }
}

[Serializable]
public class Deliver
{
    public InventoryItem inventoryItem;
    public int requestedItemAmount;
}