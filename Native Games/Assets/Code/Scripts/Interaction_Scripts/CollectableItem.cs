using UnityEngine;

public class CollectableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItem inventoryItem;
    [SerializeField] private CollectSound collectSound;

    public InteractionTrigger InteractionTrigger { get; }
    public InteractionType InteractionType { get; } = InteractionType.Collect;

    public void SetEnable()
    {
        gameObject.SetActive(true);
    }

    public void SetDisable()
    {
        gameObject.SetActive(false);
    }

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        UIManager.Instance.EnableInteractPopUp();
    }

    public void Interaction()
    {
        if (InventoryManager.Instance.Collect(this.gameObject, inventoryItem))
        {
            UIManager.Instance.DisableInteractPopUp();
            collectSound?.PlayCollectSound();
            Invoke(nameof(SetDisable), 0.25f);
        }
    }

    public void InteractionCancel()
    {

    }

    public void InteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
    }
}
