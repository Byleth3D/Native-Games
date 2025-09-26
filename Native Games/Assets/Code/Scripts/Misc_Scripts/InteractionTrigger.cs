using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class InteractionTrigger : MonoBehaviour
{
    [SerializeField] private InteractionType interactionType = InteractionType.ObjectPush;

    public IInteractable Interactable { get; private set; }
    public InteractionType InteractableType { get => interactionType; }

    private List<Collider> triggers = new();

    private void Awake()
    {
        Interactable = GetComponentInParent<IInteractable>();
        if (triggers.Count == 0) gameObject.GetComponents(triggers);
    }

    private void OnValidate()
    {
        gameObject.GetComponents(triggers);
    }

    public void TriggerEnter()
    {
        Debug.Log("Enter");
        Interactable?.OnInteractionEnter();
    }

    public void TriggerInteract(Vector3 direction)
    {
        Interactable?.OnInteract(direction);
    }

    public void TriggerInteractCancel()
    {
        Interactable?.OnInteractCancel();
    }

    public void TriggerExit()
    {
        Debug.Log("Exit");
        Interactable?.OnInteractionExit();
    }
}
