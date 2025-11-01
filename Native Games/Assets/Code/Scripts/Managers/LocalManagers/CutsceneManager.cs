using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI.Extensions.Tweens;

public class CutsceneManager : LocalSingleton<CutsceneManager>
{
    [SerializeField] private List<CheckpointTieCutscene> checkpointTieCutscenes;
    [SerializeField] private List<OnDemandCutscene> onDemandCutscenes;
    private Cutscene currentCutscene;


    protected override void Awake()
    {
        base.Awake();
        DisableAllOnDemandCutscenes();
        DisableAllCheckpointTieCutscenes();
    }

    private void Start()
    {
        int currentCheckpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;
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

    public void PlayeCheckpointTieCutscene(int checkpointIndex)
    {
        if (checkpointIndex == -1)
        {
            return;
        }

        CheckpointTieCutscene cutscene = FindCheckpointTieCutscene(checkpointIndex);

        if (cutscene == null)
        {
            return;
        }

        if (!ScreenFadeManager.Instance.IsFading)
        {
            ScreenFadeManager.Instance.ResetConfig();
            ScreenFadeManager.Instance.RequestFadeIn();
        }

        PlayCurrentCutscene(cutscene);
    }

    private OnDemandCutscene FindOnDemandCutscene(GameObject cutsceneObject)
    {
        foreach (OnDemandCutscene cutscene in onDemandCutscenes)
        {
            if (cutscene.cutsceneObject != cutsceneObject)
            {
                continue;
            }

            return cutscene;
        }

        return null;
    }

    private CheckpointTieCutscene FindCheckpointTieCutscene(int checkpointIndex)
    {
        foreach (CheckpointTieCutscene cutscene in checkpointTieCutscenes)
        {
            if (cutscene.checkpointIndex != checkpointIndex)
            {
                continue;
            }

            return cutscene;
        }

        return null;
    }

    public void PlayOnDemandCutscene(GameObject cutsceneObject)
    {
        if (cutsceneObject == null)
        {
            return;
        }

        OnDemandCutscene cutscene = FindOnDemandCutscene(cutsceneObject);

        if (cutscene == null)
        {
            return;
        }

        PlayCurrentCutscene(cutscene);
    }

    public void PlayCurrentCutscene(Cutscene cutscene)
    {
        currentCutscene = cutscene;
        currentCutscene.cutsceneObject.SetActive(true);
        currentCutscene.cutscenePlayableDirector.Play();

        float cutsceneDuration = currentCutscene.cutsceneDuration;

        InputManager.Instance.DisableGameInputs();
        Invoke(nameof(SkipCutscene), cutsceneDuration);
    }

    public void SkipCutscene()
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        if (currentCutscene == null)
        {
            return;
        }

        CancelInvoke(nameof(SkipCutscene));

        StopCoroutine(StopCurrentCutsceneAsync());
        StartCoroutine(StopCurrentCutsceneAsync());
    }

    public IEnumerator StopCurrentCutsceneAsync()
    {
        float arbitraryValue = 10000f;

        Cutscene cutscene = currentCutscene;
        currentCutscene = null;

        if (cutscene.GetType() == typeof(CheckpointTieCutscene))
        {
            int checkpointTieCutscenesCount = checkpointTieCutscenes.Count;

            if (cutscene == checkpointTieCutscenes[checkpointTieCutscenesCount - 1]
                && checkpointTieCutscenesCount > 1)
            {
                DisableCutscene(cutscene, arbitraryValue);
            }
            else
            {
                ScreenFadeManager.Instance.SetConfig("CutsceneStop");
                ScreenFadeManager.Instance.RequestFadeOut();

                while (ScreenFadeManager.Instance.IsFading)
                {
                    yield return null;
                }

                DisableCutscene(cutscene, arbitraryValue);
                CheckpointManager.Instance.CheckpointTeleport();

                ScreenFadeManager.Instance.RequestFadeIn();
            }
        }
        else
        {
            DisableCutscene(cutscene, arbitraryValue);
        }
    }

    private void DisableCutscene(Cutscene cutscene, float arbitraryValue)
    {
        Debug.Log("Disabled Cutscene");
        InputManager.Instance.EnableGameInputs();
        cutscene.cutscenePlayableDirector.time = arbitraryValue;
        cutscene.cutsceneObject.SetActive(false);
    }
}

[Serializable]
public abstract class Cutscene
{
    public GameObject cutsceneObject;
    public PlayableDirector cutscenePlayableDirector;
    public float cutsceneDuration;
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
