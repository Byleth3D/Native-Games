using UnityEngine;
using UnityEngine.Events;

public class DeliverableObject : MonoBehaviour, IInteractable
{
    [SerializeField] public int requestedAmountToDeliver = 3;
    [SerializeField] private UnityEvent OnInteraction;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Deliver;

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        UIManager.Instance.EnableInteractPopUp();
    }

    public void Interaction()
    {
        UIManager.Instance.DisableInteractPopUp();
        gameObject.SetActive(false);
        OnInteraction?.Invoke();
    }

    public void InteractionCancel()
    {
        UIManager.Instance.EnableInteractFailedPopUp();
    }

    public void InteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
        UIManager.Instance.DisableInteractFailedPopUp();
    }
}
