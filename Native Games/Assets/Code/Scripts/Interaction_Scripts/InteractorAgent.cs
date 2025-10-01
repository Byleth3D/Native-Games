using EditorAttributes;
using UnityEngine;

public abstract class InteractorAgent : Interactor
{
    [Header("Motion")]
    [ShowInInspector] protected Vector3 velocity;

    public Vector3 GetVelocity() => velocity;
    public Vector3 GetHorizontalVelocity() => velocity.WithoutY();
    public Vector3 GetVerticalVelocity() => velocity.WithY();
}
