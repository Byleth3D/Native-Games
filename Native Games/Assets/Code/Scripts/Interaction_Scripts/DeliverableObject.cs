using UnityEngine;

public class DeliverableObject : Interactable
{
    [SerializeField] public int requestedAmountToDeliver = 3;
    [SerializeField] public GameObject gate;
    private InteractorSystem interactor;

    public override void OnInteraction()
    {
        Debug.Log($"{gameObject.name}_OnInteraction");
        Destroy(gate);
    }

    public override void OnInteractionCanceled()
    {
        Debug.Log($"{gameObject.name}_OnInteractionCanceled");
    }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        interactor = interactionTrigger.Interactor as InteractorSystem;
        Debug.Log($"{gameObject.name}_OnInteractionEnter");
    }

    public override void OnInteractionExit()
    {
        interactor = null;
    }
}
