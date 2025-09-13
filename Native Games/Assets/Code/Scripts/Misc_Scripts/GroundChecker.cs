using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float circleRadius = 0.5f;
    [SerializeField] private float groundDistance = 0.01f;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private Collider collider3D;

    public bool IsGrounded { get; private set; }
    public bool PreviousGrounded { get; private set; }

    private void Update()
    {
        PreviousGrounded = IsGrounded;

        Vector3 origin;

        if (collider3D)
        {
            origin = collider3D.bounds.center - Vector3.up * (circleRadius - groundDistance);
        }
        else
        {
            origin = transform.position + Vector3.up * (circleRadius - groundDistance);
        }

        Collider[] hits = Physics.OverlapSphere(origin, circleRadius, groundLayers);

        IsGrounded = hits.Length > 0 ? true : false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 origin;

        if (collider3D)
        {
            origin = collider3D.bounds.center - Vector3.up * (circleRadius - groundDistance);
        }
        else
        {
            origin = transform.position + Vector3.up * (circleRadius - groundDistance);
        }
        Gizmos.DrawSphere(origin, circleRadius);
    }
}
