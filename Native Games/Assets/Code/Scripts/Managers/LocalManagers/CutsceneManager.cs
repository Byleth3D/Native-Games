using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : LocalSingleton<CutsceneManager>
{
    [SerializeField] private List<CheckpointTieCutscene> checkpointTieCutscenes;
    [SerializeField] private List<OnDemandCutscene> onDemandCutscenes;
    private Cutscene currentCutscene;

    private IEnumerator Start()
    {
        int currentCheckpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;

        while (CheckpointManager.Instance.CurrentCheckpointIndex == -1)
        {
            yield return null;
            currentCheckpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;
        }

        DisableAllOnDemandCutscenes();
        DisableAllCheckpointTieCutscenes();
        PlayeCheckpointTieCutscene(currentCheckpointIndex);
    }

    private void DisableAllOnDemandCutscenes()
    {
        foreach (OnDemandCutscene onDemandCutscene in onDemandCutscenes)
        {
            onDemandCutscene.cutsceneObject.SetActive(false);
        }
    }

    private void DisableAllCheckpointTieCutscenes()
    {
        foreach (CheckpointTieCutscene checkpointTieCutscene in checkpointTieCutscenes)
        {
            checkpointTieCutscene.cutsceneObject.SetActive(false);
        }
    }

    private void PlayeCheckpointTieCutscene(int checkpointIndex)
    {
        foreach (CheckpointTieCutscene checkpointTieCutscene in checkpointTieCutscenes)
        {
            if (checkpointTieCutscene.checkpointIndex != checkpointIndex)
            {
                continue;
            }

            currentCutscene = checkpointTieCutscene;
            currentCutscene.cutsceneObject.SetActive(true);
        }
    }
}

[Serializable]
public abstract class Cutscene
{
    public GameObject cutsceneObject;
    public PlayableDirector cutscenePlayableDirector;
}

[Serializable]
public class OnDemandCutscene : Cutscene
{

}

[Serializable]
public class CheckpointTieCutscene : Cutscene
{
    public int checkpointIndex;
}
