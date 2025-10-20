using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float groundDistance = 0.01f;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private CapsuleCollider collider3D;

    private RaycastHit hitInfo;

    public event Action OnGroundEnter;
    public event Action OnGroundExit;

    public bool IsGrounded { get; private set; }
    public bool PreviousGrounded { get; private set; }

    private void FixedUpdate()
    {
        PreviousGrounded = IsGrounded;
        Check();
        Respond();
    }

    private void Check()
    {
        Vector3 origin = collider3D.transform.TransformPoint(collider3D.center);
        IsGrounded = Physics.Raycast(origin, Vector3.down, out hitInfo, groundDistance, groundLayers);
    }

    private void Respond()
    {
        if (!PreviousGrounded && IsGrounded)
        {
            OnGroundEnter?.Invoke();
        }
        else if (PreviousGrounded && !IsGrounded)
        {
            OnGroundExit?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Vector3 origin = collider3D.transform.TransformPoint(collider3D.center);
        Vector3 end = Vector3.zero;

        if (IsGrounded)
        {
            end = hitInfo.point;
        }
        else
        {
            end = origin + Vector3.down * groundDistance;
        }

        Debug.DrawLine(origin, end, Gizmos.color);
        Gizmos.DrawSphere(end, 0.1f);
    }
}
