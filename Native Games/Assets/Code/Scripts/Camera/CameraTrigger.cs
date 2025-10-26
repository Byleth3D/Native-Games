using EditorAttributes;
using UnityEngine;

public enum Target
{
    A, B
}

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private bool isOneWay = false;
    [SerializeField] private int innerIndex = 0;
    [SerializeField, DisableField(nameof(isOneWay))] private int outerIndex = 0;
    private PlayerController playerController;

    public bool IsOneWay => isOneWay;

    private void OnTriggerExit(Collider other)
    {
        Vector3 triggerZoneForward = gameObject.transform.forward;

        playerController = other.gameObject.GetComponent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        Vector3 playerPosition = other.gameObject.transform.position;
        Vector3 relativePlayerPosition = transform.InverseTransformPoint(playerPosition);

        if (relativePlayerPosition.z <= 0.0f)
        {
            if (isOneWay)
            {
                return;
            }

            CameraManager.Instance.SetParameters(outerIndex);
        }
        else
        {
            CameraManager.Instance.SetParameters(innerIndex);
            enabled = false;
        }
    }
}
