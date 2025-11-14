using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : LocalSingleton<CutsceneManager>
{
    [SerializeField] private List<CheckpointTieCutscene> checkpointTieCutscenes;
    [SerializeField] private List<OnDemandCutscene> onDemandCutscenes;
    public Cutscene CurrentCutscene { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DisableCutsceneInList(onDemandCutscenes);
        DisableCutsceneInList(checkpointTieCutscenes);
    }

    private void Start()
    {
        int currentCheckpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex;
        PlayeCheckpointTieCutscene(currentCheckpointIndex);
    }

    private void DisableCutsceneInList<T>(List<T> cutscenes) where T : Cutscene
    {
        foreach (Cutscene cutscene in cutscenes)
        {
            cutscene.cutsceneObject.SetActive(false);
            cutscene.cutscenePlayableDirector.time = 10000.0;
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

        if (!ScreenFader.Instance.IsFading && cutscene.checkpointIndex > 0)
        {
            ScreenFader.Instance.Fade("Cutscene", FadeType.FadeIn).Forget();
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
        CurrentCutscene = cutscene;
        CurrentCutscene.cutscenePlayableDirector.time = 0.0;
        CurrentCutscene.cutsceneObject.SetActive(true);
        CurrentCutscene.cutscenePlayableDirector.Play();

        float cutsceneDuration = CurrentCutscene.cutsceneDuration;

        InputManager.Instance.DisableGameInputs();
        Invoke(nameof(SkipCutscene), cutsceneDuration);
    }

    public void SkipCutscene()
    {
        if (ScreenFader.Instance.IsFading)
        {
            return;
        }

        if (CurrentCutscene == null)
        {
            return;
        }

        CancelInvoke(nameof(SkipCutscene));

        StopCurrentCutsceneAsync().Forget();
    }

    public async UniTask StopCurrentCutsceneAsync()
    {
        float arbitraryValue = 10000f;

        Cutscene cutscene = CurrentCutscene;
        CurrentCutscene = null;

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
                await ScreenFader.Instance.Fade("Cutscene", FadeType.FadeOut);

                DisableCutscene(cutscene, arbitraryValue);
                CheckpointManager.Instance.CheckpointTeleport();

                ScreenFader.Instance.Fade("Cutscene", FadeType.FadeIn).Forget();
            }
        }
        else
        {
            DisableCutscene(cutscene, arbitraryValue);
        }
    }

    private void DisableCutscene(Cutscene cutscene, float arbitraryValue)
    {
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
