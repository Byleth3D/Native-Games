using UnityEngine;

public class CollectableItem : Interactable
{
    private InteractionTrigger interactionTrigger;

    public override void OnInteraction()
    {
        //Debug.Log($"{gameObject.name}_OnInteraction");
        Destroy(gameObject, 0.25f);
    }

    public override void OnInteractionCanceled()
    {
        //Debug.Log($"{gameObject.name}_OnInteractionCanceled");
    }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        //Debug.Log($"{gameObject.name}_OnInteractionEnter");
    }

    public override void OnInteractionExit()
    {
        //Debug.Log($"{gameObject.name}_OnInteractionExit ");
    }
}
