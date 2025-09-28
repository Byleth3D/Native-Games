using System;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public IInteractable Interactable { get; private set; }
    public IInteractorAgent Interactor { get; private set; }

    private void Awake()
    {
        Interactable = GetComponentInParent<IInteractable>();
    }

    public void TriggerEnter(IInteractorAgent interactor)
    {
        Interactor = interactor;
        Interactable?.OnInteractionEnter(interactor);
    }

    public void TriggerInteract()
    {
        Interactable?.OnInteract();
    }

    public void TriggerInteractCancel()
    {
        Interactable?.OnInteractCancel();
    }

    public void TriggerExit()
    {
        Interactor = null;
        Interactable?.OnInteractionExit();
    }
}
