using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CameraTriggerZone[] cameraTriggerZones;
    private CameraTriggerZone closestCameraTriggerZone;

    private void Awake()
    {
        float closestDistance = 0.0f;

        for (int i = 0; i < cameraTriggerZones.Length; i++)
        {
            Vector3 cameraTriggerPosition = cameraTriggerZones[i].transform.position.WithoutY();
            float distance = Vector3.Distance(transform.position, cameraTriggerPosition);

            if (i == 0 || distance < closestDistance)
            {
                closestCameraTriggerZone = cameraTriggerZones[i];
                closestDistance = distance;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.Instance.SetActiveCheckpoint(this);
    }

    public void ResetCameraTriggersAlong()
    {
        foreach (CameraTriggerZone cameraTriggerZone in cameraTriggerZones)
        {
            bool closest = cameraTriggerZone == closestCameraTriggerZone ? true : false;

            cameraTriggerZone.ResetCameraTrigger(closest);
        }
    }
}
