using UnityEngine;

public class CheatsManager : LocalSingleton<CheatsManager>
{
    private void Update()
    {
        if (!InputManager.Instance.Cheats.ModifierPressed)
        {
            return;
        }

        if (InputManager.Instance.Cheats.TeleportPreviousPressed)
        {
            int checkpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;
            CheckpointManager.Instance.CheckpointTeleport(checkpointIndex - 1);
            return;
        }

        if (InputManager.Instance.Cheats.TeleportNextPressed)
        {
            int checkpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;
            CheckpointManager.Instance.CheckpointTeleport(checkpointIndex + 1);
            return;
        }
    }
}
