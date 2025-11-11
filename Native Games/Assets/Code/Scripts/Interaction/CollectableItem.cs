using UnityEngine;
using UnityEngine.Events;

public class CollectableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItem inventoryItem;
    [SerializeField] private CollectSound collectSound;
    [Space]
    [SerializeField] private UnityEvent OnCollect;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Collect;

    public void SetEnable()
    {
        gameObject.SetActive(true);
    }

    public void SetDisable()
    {
        InteractionTrigger.TriggerForceInteractExit();
        gameObject.SetActive(false);
    }

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        GameplayUIManager.Instance.popupManager.DisableActivePopup();
        GameplayUIManager.Instance.popupManager.EnablePopup("Interact");
        InteractionTrigger = interactionTrigger;
    }

    public void Interaction()
    {
        if (InventoryManager.Instance.Collect(this.gameObject, inventoryItem))
        {
            GameplayUIManager.Instance.popupManager.DisablePopup("Interact");
            collectSound?.PlayCollectSound();
            OnCollect?.Invoke();
            ObjectStateManager.Instance.SetAsCollected(gameObject);
            Invoke(nameof(SetDisable), 0.25f);
        }
        else
        {
            InteractionTrigger.TriggerInteractCancel(false);
        }
    }

    public void InteractionCancel()
    {

    }

    public void InteractionExit()
    {
        GameplayUIManager.Instance.popupManager.DisablePopup("Interact");
    }
}
