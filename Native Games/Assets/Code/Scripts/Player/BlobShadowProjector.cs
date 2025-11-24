using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlobShadowProjector : MonoBehaviour
{
    [SerializeField] private Vector3 initialLocalPosition = Vector3.zero;
    [SerializeField] private DecalProjector projector;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        ResetPosition();
    }

    private void Update()
    {
        if (groundChecker.IsGrounded)
        {
            projector.enabled = true;
            ResetPosition();
        }
        else
        {
            Project();
        }
    }

    private void ResetPosition()
    {
        projector.transform.localPosition = initialLocalPosition;
    }

    private void Project()
    {
        Vector3 origin = transform.TransformPoint(initialLocalPosition);
        bool hit = Physics.Raycast(origin, Vector3.down, out RaycastHit hitInfo, 1000f, layerMask);

        if (hit)
        {
            projector.enabled = true;
            projector.transform.position = hitInfo.point;
        }
        else
        {
            projector.enabled = false;
            ResetPosition();
        }
    }
}