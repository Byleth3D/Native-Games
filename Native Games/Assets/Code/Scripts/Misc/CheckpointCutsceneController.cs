using EditorAttributes;
using UnityEngine;

public class CheckpointCutsceneController : MonoBehaviour
{
    [SerializeField] private int checkpointIndex = -1;

    private void Update()
    {
        if (checkpointIndex > -1)
        {
            int currentSaveIndex = SaveManager.Instance.CurrentSaveIndex;

            if (currentSaveIndex == -1)
            {
                return;
            }

            int currentCheckpointIndex = SaveManager.Instance.SaveFiles[currentSaveIndex].checkpointIndex;

            if (currentCheckpointIndex != checkpointIndex)
            {
                this.gameObject.SetActive(false);
                return;
            }
        }
    }
}
