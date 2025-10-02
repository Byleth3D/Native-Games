using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float groundDistance = 0.01f;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private CapsuleCollider collider3D;

    private RaycastHit hitInfo;

    public event Action OnGroundEnter;
    public event Action OnGroundExit;

    public bool IsGrounded { get; private set; }
    public bool PreviousGrounded { get; private set; }

    private void Update()
    {
        PreviousGrounded = IsGrounded;
        Check();
        Respond();
    }

    private void Check()
    {
        Vector3 origin = transform.position + Vector3.up * collider3D.radius;

        bool hit = Physics.SphereCast(origin, sphereRadius, Vector3.down, out hitInfo, 1000f, groundLayers, QueryTriggerInteraction.Ignore);

        if (!hit)
        {
            IsGrounded = false;
        }
        else
        {
            IsGrounded = transform.position.y - hitInfo.point.y <= groundDistance ? true : false;
        }
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

        Vector3 origin = transform.position + Vector3.up * sphereRadius;
        origin = IsGrounded ? origin : origin + Vector3.down * 1000f;

        Gizmos.DrawSphere(origin, sphereRadius);
    }
}
