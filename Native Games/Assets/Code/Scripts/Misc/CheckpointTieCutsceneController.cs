using EditorAttributes;
using UnityEngine;

public class CheckpointTieCutsceneController : MonoBehaviour
{
    [SerializeField] private int checkpointIndex = -1;

    private void Start()
    {
        if (checkpointIndex > -1)
        {
            if (CheckpointManager.Instance.CurrentCheckpointIndex != checkpointIndex)
            {
                this.gameObject.SetActive(false);
                return;
            }
        }
    }
}
