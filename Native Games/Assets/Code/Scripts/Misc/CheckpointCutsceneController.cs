using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CheckpointCutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private int checkpointIndex = -1;

    private IEnumerator Start()
    {
        if (checkpointIndex > -1)
        {
            int currentSaveIndex = SaveManager.Instance.CurrentSaveIndex;

            while (currentSaveIndex == -1)
            {
                yield return null;
                currentSaveIndex = SaveManager.Instance.CurrentSaveIndex;
            }

            int currentCheckpointIndex = SaveManager.Instance.Saves[currentSaveIndex].checkpointIndex;

            if (currentCheckpointIndex != checkpointIndex)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                playableDirector.enabled = true;
            }
        }
    }
}
