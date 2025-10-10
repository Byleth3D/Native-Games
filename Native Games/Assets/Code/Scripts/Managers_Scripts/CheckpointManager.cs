using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] private List<Checkpoint> checkpoints = new();

    private Checkpoint currentCheckpoint;
    private int currentCheckpointIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        if (checkpoints.Count > 0)
        {
            currentCheckpoint = checkpoints[0];
        }
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoints.Count == 0 || checkpoint == currentCheckpoint)
        {
            return;
        }

        bool isRegistered = checkpoints.Contains(checkpoint);

        if (!isRegistered)
        {
            return;
        }

        int checkpointIndex = checkpoints.IndexOf(checkpoint);

        if (checkpointIndex <= currentCheckpointIndex)
        {
            return;
        }

        currentCheckpoint = checkpoint;
        currentCheckpointIndex = checkpointIndex;

        if (currentCheckpointIndex == checkpoints.Count - 1)
        {
            SceneLoader.Instance.LoadNextScene();
        }
    }

    public void CheckpointTeleport()
    {
        if (checkpoints.Count == 0)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = null;

        if (playerObject == null)
        {
            return;
        }

        playerController = playerObject.GetComponent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        playerController.SetAsAlive();
        playerController.Teleport(currentCheckpoint.transform.position);

        currentCheckpoint.ResetCameraTriggersAlong();
    }

    public void CheckpointTeleport(int checkPointIndex)
    {
        if (checkpoints.Count == 0 || checkPointIndex >= checkpoints.Count)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = null;

        if (playerObject == null)
        {
            return;
        }

        playerController = playerObject.GetComponent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        currentCheckpointIndex = checkPointIndex;
        playerController.SetAsAlive();
        playerController.Teleport(checkpoints[checkPointIndex].transform.position);
    }
}
