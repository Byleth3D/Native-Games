using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IInteractable
{
    public InteractionType Interaction { get; } = InteractionType.ObjectPush;
    private IInteractorAgent interactorController;
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

        float speed = interactorController.GetHorizontalVelocity().magnitude;
        Vector3 moveDirection = interactorController.GetHorizontalVelocity().normalized;

        rigidBody.AddForce(moveDirection * speed - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    public void OnInteractionEnter(IInteractorAgent interactorController)
    {
        if (interactorController == null) return;
        this.interactorController = interactorController as IInteractorAgent;
    }

    public void OnInteract()
    {
        TurnRigidbodyDynamic();
        Push();
    }

    public void OnInteractCancel()
    {
        TurnRigidbodyKinematic();
    }

    public void OnInteractionExit()
    {
        interactorController = null;
        TurnRigidbodyKinematic();
    }
}
