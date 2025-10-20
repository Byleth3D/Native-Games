using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private AsyncOperation loadingOperation;

    public void LoadScene(int nextSceneIndex)
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    public void LoadScene(string nextSceneName)
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

        Debug.Log($"Next Scene: {scenePath.Substring(sceneNameStart, sceneNameLength)}");

        return scenePath.Substring(sceneNameStart, sceneNameLength);
    }

    public void StartNewGameLoadingChain()
    {
        StopCoroutine(NewGameLoadingChain());
        StartCoroutine(NewGameLoadingChain());
    }

    public void StartLastGameLoadingChain()
    {
        StopCoroutine(LastGameLoadingChain());
        StartCoroutine(LastGameLoadingChain());
    }

    private IEnumerator NewGameLoadingChain()
    {
        Debug.Log("Started");

        ScreenFadeManager.Instance.SetConfig("Loading");

        ScreenFadeManager.Instance.RequestFadeOut();

        while (ScreenFadeManager.Instance.IsFading)
        {
            yield return null;
        }

        SaveManager.Instance.NewSaveGameFromMenu();

        List<SaveData> saveFiles = SaveManager.Instance.SaveFiles;
        int currentSaveIndex = SaveManager.Instance.CurrentSaveIndex;

        string sceneName = saveFiles[currentSaveIndex].activeSceneName;

        LoadScene(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Tela de loading começa sumir quando o carregamento da cena termina
        ScreenFadeManager.Instance.RequestFadeIn(() => ScreenFadeManager.Instance.ResetConfig());//Fader
        yield return null;
    }

    private IEnumerator LastGameLoadingChain()
    {
        ScreenFadeManager.Instance.SetConfig("Loading");
        ScreenFadeManager.Instance.RequestFadeOut();

        while (ScreenFadeManager.Instance.IsFading)
        {
            yield return null;
        }

        List<SaveData> saveFiles = SaveManager.Instance.SaveFiles;
        int lastSaveIndex = saveFiles.Count - 1;

        string sceneName = saveFiles[lastSaveIndex].activeSceneName;

        LoadScene(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        SaveManager.Instance.LoadLastGame();
        // Tela de loading começa sumir quando o carregamento da cena termina
        ScreenFadeManager.Instance.RequestFadeIn(() => ScreenFadeManager.Instance.ResetConfig());//Fader
        yield return null;
    }
}
