using UnityEngine;

public class DeliverableObject : Interactable
{
    [SerializeField] public int requestedAmountToDeliver = 3;
    [SerializeField] public GameObject gate;
    private InteractorSystem interactor;

    public override void OnInteraction()
    {
        UIManager.Instance.DisableInteractPopUp();
        Destroy(gate);
        gameObject.SetActive(false);
    }

    public override void OnInteractionCanceled()
    {
        UIManager.Instance.EnableInteractFailedPopUp();
    }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        interactor = interactionTrigger.Interactor as InteractorSystem;
        UIManager.Instance.EnableInteractPopUp();
    }

    public override void OnInteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
        UIManager.Instance.DisableInteractFailedPopUp();
    }
}
