using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public void LoadScene(int nextSceneIndex)
    {
        SceneManager.LoadSceneAsync(nextSceneIndex);
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

        ScreenFadeManager.Instance.ResetValues();
        ScreenFadeManager.Instance.RequestFadeOut(() => LoadScene(nextSceneIndex));
    }

    public void ReloadScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount == 0 && ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        ScreenFadeManager.Instance.ResetValues();
        ScreenFadeManager.Instance.RequestFadeOut(() => LoadScene(currentSceneIndex));
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
