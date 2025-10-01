using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : Interactable
{
    private InteractorAgent interactorAgent;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        TurnRigidbodyKinematic();
    }

    private void OnValidate()
    {
        rigidBody = GetComponent<Rigidbody>();
        TurnRigidbodyKinematic();
    }

    public void TurnRigidbodyDynamic()
    {
        if (rigidBody == null || !rigidBody.isKinematic) return;

        rigidBody.isKinematic = false;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void TurnRigidbodyKinematic()
    {
        if (rigidBody == null || rigidBody.isKinematic) return;

        rigidBody.isKinematic = true;
        rigidBody.interpolation = RigidbodyInterpolation.None;
    }

    public void Push()
    {
        if (rigidBody == null) return;

        TurnRigidbodyDynamic();

        float speed = interactorAgent.GetHorizontalVelocity().magnitude;
        Vector3 moveDirection = interactorAgent.GetHorizontalVelocity().normalized;

        rigidBody.AddForce(moveDirection * speed - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    public override void OnInteractionEnter(InteractionTrigger interactionTrigger)
    {
        interactorAgent = interactionTrigger.Interactor as InteractorAgent;
    }

    public override void OnInteraction()
    {
        TurnRigidbodyDynamic();
        Push();
    }

    public override void OnInteractionCanceled()
    {
        TurnRigidbodyKinematic();
    }

    public override void OnInteractionExit()
    {
        interactorAgent = null;
        TurnRigidbodyKinematic();
    }
}
