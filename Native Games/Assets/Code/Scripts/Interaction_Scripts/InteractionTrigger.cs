using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Interactable Interactable { get; private set; }
    public Interactor Interactor { get; private set; }
    public Collider ActiveTrigger { get; private set; }
    public Collider[] triggers { get; private set; }
    public bool HasInteraction { get; private set; } = false;

    private void Awake()
    {
        Interactable = GetComponentInParent<Interactable>();
        triggers = GetComponents<Collider>();
    }

    public void OnTriggerInteract()
    {
        HasInteraction = true;
        Interactable?.OnInteraction();
    }

    public void OnTriggerInteractCanceled()
    {
        HasInteraction = false;
        Interactable?.OnInteractionCanceled();
    }

    private Interactor GetCompatibleInteractor(List<Interactor> interactors)
    {
        InteractableType interactableDefinition = Interactable.GetInteractableDefinition();
        InteractorType interactorFilter = Interactable.GetInteractorFilter();

        foreach (Interactor interactor in interactors)
        {
            if (interactorFilter == interactor.GetInteractorDefinition() && interactor.HasInteractableInFilter(interactableDefinition))
            {
                return interactor;
            }
        }
        return null;
    }

    private Interactor TryGetActiveInteractor(List<Interactor> interactors)
    {
        foreach (Interactor interactor in interactors)
        {
            if (this.Interactor == interactor)
            {
                return interactor;
            }
        }
        return null;
    }

    private Collider GetActiveCollider(Interactor interactor)
    {
        Vector3 interactorPosition = interactor.gameObject.transform.position;
        interactorPosition.y = 0.0f;

        Collider colliderToReturn = null;
        float minSqrDistance = 0.0f;

        foreach (Collider trigger in triggers)
        {
            if (colliderToReturn == null)
            {
                colliderToReturn = trigger;
                minSqrDistance = (trigger.bounds.center - interactorPosition).sqrMagnitude;
                continue;
            }

            float sqrDistance = (trigger.bounds.center - interactorPosition).sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                colliderToReturn = trigger;
                minSqrDistance = sqrDistance;
            }
        }

        return colliderToReturn;
    }

    private void OnTriggerEnter(Collider interactorCollider)
    {
        if (HasInteraction) return;
        List<Interactor> interactors = new();
        interactorCollider.GetComponents(interactors);

        if (interactors.Count == 0) return;

        Interactor activeInteractor = GetCompatibleInteractor(interactors);

        if (activeInteractor == null) return;

        ActiveTrigger = GetActiveCollider(activeInteractor);

        this.Interactor = activeInteractor;
        Interactor.OnInteractionEnter(this);

        Interactable.OnInteractionEnter(this);
    }

    private void OnTriggerExit(Collider interactorCollider)
    {
        if (HasInteraction) return;
        List<Interactor> interactors = new();
        interactorCollider.GetComponents(interactors);

        if (interactors.Count == 0) return;

        Interactor activeInteractor = TryGetActiveInteractor(interactors);

        if (activeInteractor == null) return;

        ActiveTrigger = null;

        Interactor.OnInteractionExit();
        Interactor = null;

        Interactable.OnInteractionExit();
    }
}