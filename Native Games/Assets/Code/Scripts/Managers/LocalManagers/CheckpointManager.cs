using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class CheckpointManager : LocalSingleton<CheckpointManager>
{
    [SerializeField] private List<Checkpoint> checkpoints = new();
    [Space]
    [SerializeField] private UnityEvent OnLastCheckpointReached;
    private Checkpoint currentCheckpoint;
    public int CurrentCheckpointIndex { get; private set; } = -1;

    private void Start()
    {
        int curentSaveIndex = SaveManager.Instance.CurrentSaveIndex;
        int currentCheckpointIndex = SaveManager.Instance.SaveFiles[curentSaveIndex].checkpointIndex;
        ForceSetCheckpoint(currentCheckpointIndex);
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

        if (checkpointIndex == 0)
        {
            return;
        }

        CutsceneManager.Instance.PlayeCheckpointTieCutscene(checkpointIndex);

        if (CurrentCheckpointIndex == checkpoints.Count - 1)
        {
            OnLastCheckpointReached?.Invoke();
            return;
        }

        SaveManager.Instance.NewSaveGameFromCheckpoint();
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
        CameraManager.Instance.ResetParameters(currentCheckpoint);
    }

    public void CheckpointTeleport(int checkpointIndex)
    {
        if (checkpoints.Count == 0 || checkpointIndex >= checkpoints.Count)
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

        CurrentCheckpointIndex = checkpointIndex;
        playerController.SetAsAlive();
        playerController.Teleport(checkpoints[checkpointIndex].transform.position);
        CameraManager.Instance.ResetParameters(currentCheckpoint);
    }

    public void ForceSetCheckpoint(int index, bool teleport = true)
    {
        if (index >= checkpoints.Count || checkpoints.Count == 0)
        {
            Debug.LogError("Array Out of Bounds!");
            return;
        }

        currentCheckpoint = checkpoints[index];
        CurrentCheckpointIndex = index;

        if (teleport)
        {
            CheckpointTeleport(index);
        }
    }
}
