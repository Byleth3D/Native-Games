using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float circleRadius = 0.5f;
    [SerializeField] private float groundDistance = 0.01f;
    [SerializeField] private LayerMask groundLayers;

    public bool IsGrounded { get; private set; }
    public bool PreviousGrounded{ get; private set; }

    private void Update()
    {
        PreviousGrounded = IsGrounded;

        Vector3 position = transform.position + Vector3.up * (circleRadius - groundDistance);
        Collider[] hits = Physics.OverlapSphere(position, circleRadius, groundLayers);

        IsGrounded = hits.Length > 0 ? true : false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * (circleRadius - groundDistance), circleRadius);
    }
}
