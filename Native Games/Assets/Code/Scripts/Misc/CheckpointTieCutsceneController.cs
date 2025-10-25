using EditorAttributes;
using UnityEngine;

public class CheckpointTieCutsceneController : MonoBehaviour
{
    [SerializeField] private int checkpointIndex = -1;

    private void Start()
    {
        if (checkpointIndex > -1)
        {
            int currentSaveIndex = SaveManager.Instance.CurrentSaveIndex;
            int currentCheckpointIndex = SaveManager.Instance.SaveFiles[currentSaveIndex].checkpointIndex;
            if (currentCheckpointIndex != checkpointIndex)
            {
                this.gameObject.SetActive(false);
                return;
            }
        }
    }
}
