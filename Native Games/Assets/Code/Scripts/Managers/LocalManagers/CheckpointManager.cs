using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CheckpointManager : LocalSingleton<CheckpointManager>
{
    [SerializeField] private List<Checkpoint> checkpoints = new();
    [Space]
    [SerializeField] private UnityEvent OnLastCheckpointReached;
    private Checkpoint currentCheckpoint;
    public int CurrentCheckpointIndex { get; private set; } = 0;

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

        if (checkpointIndex <= CurrentCheckpointIndex)
        {
            return;
        }

        currentCheckpoint = checkpoint;
        CurrentCheckpointIndex = checkpointIndex;

        if (CurrentCheckpointIndex == checkpoints.Count - 1)
        {
            OnLastCheckpointReached?.Invoke();
            return;
        }

        SaveManager.Instance.NewSaveGame();
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

        CurrentCheckpointIndex = checkPointIndex;
        playerController.SetAsAlive();
        playerController.Teleport(checkpoints[checkPointIndex].transform.position);

        currentCheckpoint.ResetCameraTriggersAlong();
    }

    public void ReloadCheckpointsFrom(int index)
    {
        if (index >= checkpoints.Count)
        {
            Debug.LogError("Array Out of Bounds!");
            return;
        }

        if (index < checkpoints.Count - 1)
        {
            for (int i = index + 1; i < checkpoints.Count; i++)
            {
                checkpoints[i].ResetCameraTriggersAlong();
            }
        }

        CheckpointTeleport(index);
    }
}
