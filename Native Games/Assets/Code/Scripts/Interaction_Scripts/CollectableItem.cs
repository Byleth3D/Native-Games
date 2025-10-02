using UnityEngine;

public class CollectableItem : Interactable
{
    private InteractionTrigger interactionTrigger;

    public override void OnInteraction()
    {
        UIManager.Instance.DisableInteractPopUp();
        Destroy(gameObject, 0.25f);
    }

    public override void OnInteractionCanceled()
    {
        //Debug.Log($"{gameObject.name}_OnInteractionCanceled");
    }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        UIManager.Instance.EnableInteractPopUp();
    }

    public override void OnInteractionExit()
    {
        UIManager.Instance.DisableInteractPopUp();
    }
}
