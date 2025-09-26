using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IInteractable
{
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

    public void Push(Vector3 direction)
    {
        if (rigidBody == null) return;
        TurnRigidbodyDynamic();
        rigidBody.AddForce(direction * 5f - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    public void OnInteractionEnter()
    {
    }

    public void OnInteract(Vector3 direction)
    {
        TurnRigidbodyDynamic();
        Push(direction);
    }

    public void OnInteractCancel()
    {
        TurnRigidbodyKinematic();
    }

    public void OnInteractionExit()
    {
        TurnRigidbodyKinematic();
    }
}

public interface IInteractable
{
    void OnInteractionEnter();
    void OnInteract(Vector3 direction);
    void OnInteractCancel();
    void OnInteractionExit();
}

public enum InteractionType
{
    ItemCollect,
    ItemDeliver,
    ObjectPush,
    None
}
