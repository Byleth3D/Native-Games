using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private IInteractable interactable;
    public PlayerController PlayerController { get; private set; }

    public Collider[] Triggers { get; private set; }
    public Collider ActiveTrigger { get; private set; }

    public bool HasInteraction { get; private set; } = false;

    private void Awake()
    {
        interactable = GetComponentInParent<IInteractable>();
        Triggers = GetComponents<Collider>();
    }

    public void TriggerInteract(bool persistentInteraction)
    {
        if (!HasInteraction && persistentInteraction)
        {
            HasInteraction = true;
        }

        interactable.Interaction();
    }

    public void TriggerInteractCancel(bool persistentInteraction)
    {
        if (HasInteraction && persistentInteraction)
        {
            interactable.InteractionCancel();
        }

        HasInteraction = false;
    }

    public InteractionType GetInteractionType()
    {
        return interactable.InteractionType;
    }

    private Collider GetActiveCollider(Collider actorCollider)
    {
        Vector3 actorPosition = actorCollider.attachedRigidbody.position.WithoutY();

        Collider colliderToReturn = null;
        float minSqrDistance = 0.0f;

        foreach (Collider trigger in Triggers)
        {
            if (colliderToReturn == null)
            {
                colliderToReturn = trigger;
                minSqrDistance = (trigger.bounds.center - actorPosition).sqrMagnitude;
                continue;
            }

            float sqrDistance = (trigger.bounds.center - actorPosition).sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                colliderToReturn = trigger;
                minSqrDistance = sqrDistance;
            }
        }

        return colliderToReturn;
    }

    private void OnTriggerEnter(Collider actorCollider)
    {
        if (HasInteraction)
        {
            return;
        }

        if (PlayerController == null)
        {
            PlayerController playerController = actorCollider.gameObject.GetComponent<PlayerController>();

            if (playerController == null)
            {
                return;
            }

            this.PlayerController = playerController;
        }

        if (PlayerController.InteractionTrigger != null && PlayerController.InteractionTrigger != this)
        {
            return;
        }

        ActiveTrigger = GetActiveCollider(actorCollider);

        this.PlayerController.InteractionEnter(this);

        interactable.InteractionEnter(this, PlayerController);
    }

    private void OnTriggerExit(Collider actorCollider)
    {
        if (actorCollider == null || PlayerController == null || ActiveTrigger == null)
        {
            return;
        }

        if (HasInteraction)
        {
            float sqrDistance = Vector3.Distance(ActiveTrigger.bounds.center, actorCollider.attachedRigidbody.position);

            if (sqrDistance <= PlayerController.MaxInteractionDistance)
            {
                return;
            }
        }

        HasInteraction = false;

        ActiveTrigger = null;

        PlayerController.InteractionExit();

        interactable.InteractionExit();
    }
}