using UnityEngine;

public interface IInteractorAgent
{
    Vector3 GetVelocity();
    Vector3 GetHorizontalVelocity();
    Vector3 GetVerticalVelocity();
    Vector3 GetForwardDirection();
}
