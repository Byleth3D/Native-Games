using Unity.Cinemachine;
using UnityEngine;

public class CheatsManager : LocalSingleton<CheatsManager>
{
    [SerializeField] private CinemachineCamera freeCamera;
    [SerializeField] private FreeController freeController;

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

        if (InputManager.Instance.Cheats.FreeMode && (CutsceneManager.Instance.CurrentCutscene == null
            || LoadingScreen.Instance.IsVisible))
        {
            if (freeCamera.gameObject.activeInHierarchy
                && freeController.gameObject.activeInHierarchy)
            {
                freeCamera.gameObject.SetActive(false);
                freeController.gameObject.SetActive(false);
                freeController.SetParent(toNull: false);

                InputManager.Instance.EnableGameInputs();
                return;
            }

            freeCamera.gameObject.SetActive(true);
            freeController.gameObject.SetActive(true);
            freeController.SetParent(toNull: true);

            InputManager.Instance.DisableGameInputs(includeCheats: false);

            GameplayUIManager.Instance.DisableActiveInGameMenu();
            GameplayUIManager.Instance.DisablePopup();
            return;
        }
    }
}
