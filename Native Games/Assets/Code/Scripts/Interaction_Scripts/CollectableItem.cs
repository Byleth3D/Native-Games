using UnityEngine;

public class CollectableItem : MonoBehaviour, IInteractable
{
    public InteractionTrigger InteractionTrigger { get; }
    public InteractionType InteractionType { get; } = InteractionType.Collect;

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        UIManager.Instance.EnableInteractPopUp();
    }

    public void Interaction()
    {
        Debug.Log("Interaction Collectable");
        InventoryManager.Instance.Collect();
        UIManager.Instance.DisableInteractPopUp();
        Destroy(gameObject, 0.25f);
    }

    public void InteractionCancel()
    {

    }

    public void InteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
    }
}
