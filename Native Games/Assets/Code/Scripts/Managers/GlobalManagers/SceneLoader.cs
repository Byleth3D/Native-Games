using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private AsyncOperation loadingOperation;

    private void LoadScene(int nextSceneIndex)
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    private void LoadScene(string nextSceneName)
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneName);
    }

    public void LoadNextScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount == 0)
        {
            return;
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex > sceneCount - 1)
        {
            nextSceneIndex = 0;
        }

        ScreenFadeManager.Instance.ResetConfig();
        ScreenFadeManager.Instance.RequestFadeOut(() =>
        {
            LoadScene(nextSceneIndex);
            ScreenFadeManager.Instance.RequestFadeIn();
        });
    }

    public void ReloadScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount == 0 && ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        ScreenFadeManager.Instance.ResetConfig();
        ScreenFadeManager.Instance.RequestFadeOut(() => LoadScene(currentSceneIndex));
    }

    public int GetActiveSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public string GetNextSceneName()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);

        var sceneNameStart = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
        var sceneNameEnd = scenePath.LastIndexOf(".", StringComparison.Ordinal);
        var sceneNameLength = sceneNameEnd - sceneNameStart;

        return scenePath.Substring(sceneNameStart, sceneNameLength);
    }

    public void StartLoading(LoadingType loadingType)
    {
        StopCoroutine(LoadingChain(loadingType));
        StartCoroutine(LoadingChain(loadingType));
    }

    public void StartLoading(string sceneName)
    {
        StopCoroutine(LoadingChain(sceneName));
        StartCoroutine(LoadingChain(sceneName));
    }

    private IEnumerator LoadingChain(LoadingType loadingType, int saveIndex = -1)
    {
        LoadingScreenManager.Instance.EnableLoadingScreen();

        while (!LoadingScreenManager.Instance.IsVisible)
        {
            yield return null;
        }

        List<SaveData> saveFiles = SaveManager.Instance.SaveFiles;
        string sceneName = "";

        switch (loadingType)
        {
            case LoadingType.NewGame:
                SaveManager.Instance.NewSaveGameFromMenu();
                int newSaveIndex = SaveManager.Instance.CurrentSaveIndex;
                sceneName = saveFiles[newSaveIndex].activeSceneName;
                break;

            case LoadingType.ContinueGame:
                int lastSaveIndex = saveFiles.Count - 1;
                sceneName = saveFiles[lastSaveIndex].activeSceneName;
                break;

            case LoadingType.LoadGame:
                if (saveIndex == -1)
                {
                    Debug.LogError("Index Out of Bounds");
                }

                saveFiles = SaveManager.Instance.SaveFiles;
                sceneName = saveFiles[saveIndex].activeSceneName;

                break;

            case LoadingType.NextScene:
                sceneName = GetNextSceneName();

                if (sceneName.Contains("Level"))
                {
                    goto case LoadingType.NewGame;
                }

                break;
            case LoadingType.RestartCheckpoint:
                saveIndex = SaveManager.Instance.CurrentSaveIndex;
                Debug.Log($"Save Index {saveIndex}");
                sceneName = saveFiles[saveIndex].activeSceneName;
                break;
        }

        LoadScene(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.5f);
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingType == LoadingType.ContinueGame)
        {
            SaveManager.Instance.LoadLastGame();
        }
        else if (loadingType == LoadingType.LoadGame || loadingType == LoadingType.RestartCheckpoint)
        {
            SaveManager.Instance.LoadGame(saveIndex);
        }

        LoadingScreenManager.Instance.DisableLoadingScreen();
        yield return null;
    }

    private IEnumerator LoadingChain(string sceneName)
    {
        LoadingScreenManager.Instance.EnableLoadingScreen();

        while (!LoadingScreenManager.Instance.IsVisible)
        {
            yield return null;
        }

        LoadScene(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.5f);
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        LoadingScreenManager.Instance.DisableLoadingScreen();
        yield return null;
    }
}

public enum LoadingType
{
    NewGame, ContinueGame, LoadGame, NextScene, RestartCheckpoint
}
