using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public AsyncOperation LoadingOperation { get; private set; }
    public event Action<string> OnSceneLoaded;

    protected override void Awake()
    {
        base.Awake();
    }

    private async UniTask LoadSceneAsync(LoadingType loadingType, int saveIndex = -1)
    {
        await LoadingScreen.Instance.SelfEnable();

        SaveData save = SaveManager.Instance.GetSaveForLoading(loadingType, saveIndex);

        string sceneName = save is null ?
        SceneManagerWrapper.GetNextSceneName() : save.activeSceneName;

        saveIndex = save is null ?
        saveIndex : SaveManager.Instance.GetSaveIndex(save);

        await UniTask.NextFrame();
        LoadScene(sceneName);
        LoadingOperation.allowSceneActivation = false;

        await UniTask.WaitWhile(() => LoadingOperation.progress < 0.9f);
        await UniTask.Delay(2500);

        if (save != null)
        {
            SaveManager.Instance.LoadGame(saveIndex);
        }

        LoadingOperation.allowSceneActivation = true;

        await UniTask.WaitWhile(() => SceneManagerWrapper.GetActiveSceneName() != sceneName);

        LoadingScreen.Instance.SelfDisable().Forget();

        OnSceneLoaded?.Invoke(sceneName);
    }

    private async UniTaskVoid LoadSceneAsync(string sceneName, bool withLoadingScreen = true)
    {
        int delay = 0;

        if (withLoadingScreen)
        {
            delay = 2500;
            await LoadingScreen.Instance.SelfEnable();
        }
        else
        {
            if (!SceneManagerWrapper.GetNextSceneName().Contains("Level"))
            {
                if (SceneManagerWrapper.GetActiveSceneName().Contains("GlobalManagers"))
                {
                    ScreenFader.Instance.PresetAlphaFor(FadeType.FadeIn);
                }
                else
                {
                    await ScreenFader.Instance.Fade("Default", FadeType.FadeOut);
                }
            }
        }

        await UniTask.NextFrame();

        LoadScene(sceneName);
        LoadingOperation.allowSceneActivation = false;

        await UniTask.WaitWhile(() => LoadingOperation.progress < 0.9f);
        await UniTask.Delay(delay);

        if (sceneName.Contains("Level"))
        {
            if (SaveManager.Instance.Saves.Count == 0)
            {
                SaveManager.Instance.CreateSaveGame(fromCheckpoint: false, onActiveScene: true);
            }

            SaveManager.Instance.LoadGame();
        }

        LoadingOperation.allowSceneActivation = true;

        await UniTask.WaitUntil(() => SceneManagerWrapper.GetActiveSceneName() == sceneName);

        if (withLoadingScreen)
        {
            LoadingScreen.Instance.SelfDisable().Forget();
        }
        else
        {
            ScreenFader.Instance.Fade("Default", FadeType.FadeIn).Forget();
        }

        OnSceneLoaded?.Invoke(sceneName);
    }

    private void LoadScene(string sceneName)
    {
        LoadingOperation = SceneManager.LoadSceneAsync(sceneName);
    }

    public void Load(LoadingType loadingType)
    {
        LoadSceneAsync(loadingType).Forget();
    }

    public void Load(string sceneName)
    {
        LoadSceneAsync(sceneName).Forget();
    }

    public void LoadWithoutLoadingScreen(string sceneName)
    {
        LoadSceneAsync(sceneName, withLoadingScreen: false).Forget();
    }
}

public enum LoadingType
{
    NewGame, ContinueGame, LoadGame, NextScene, RestartCheckpoint
}
