
using System;
using UnityEngine.SceneManagement;

public static class SceneManagerWrapper
{
    public static string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static int GetActiveSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static string GetNextSceneName()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);

        var sceneNameStart = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
        var sceneNameEnd = scenePath.LastIndexOf(".", StringComparison.Ordinal);
        var sceneNameLength = sceneNameEnd - sceneNameStart;

        string sceneName = scenePath.Substring(sceneNameStart, sceneNameLength);

        if (string.IsNullOrEmpty(sceneName))
        {
            return string.Empty;
        }

        return sceneName;
    }

    public static int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }
}
