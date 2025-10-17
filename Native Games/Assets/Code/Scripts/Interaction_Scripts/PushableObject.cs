using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IInteractable
{
    private Rigidbody rigidBody;
    private PlayerController playerController;

    public InteractionTrigger InteractionTrigger { get; set; }
    public InteractionType InteractionType { get; } = InteractionType.Push;

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

        float speed = playerController.Velocity.magnitude;
        Vector3 moveDirection = playerController.Velocity.normalized;

        rigidBody.AddForce(moveDirection * speed - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    public void InteractionEnter(InteractionTrigger interactionTrigger, PlayerController playerController)
    {
        this.playerController = playerController;
        GameplayUIManager.Instance.EnablePushPopUp();
    }

    public void Interaction()
    {
        TurnRigidbodyDynamic();
        Push();
        GameplayUIManager.Instance.DisablePushPopUp();
    }

    public void InteractionCancel()
    {
        rigidBody.AddForce(-rigidBody.linearVelocity, ForceMode.VelocityChange);
        TurnRigidbodyKinematic();
    }

    public void InteractionExit()
    {
        rigidBody.AddForce(-rigidBody.linearVelocity, ForceMode.VelocityChange);
        TurnRigidbodyKinematic();
        GameplayUIManager.Instance.DisablePushPopUp();
    }
}
