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

        if (nextSceneIndex > sceneCount)
        {
            nextSceneIndex = 0;
        }

        ScreenFadeManager.Instance.RequestFadeOut(() => LoadScene(nextSceneIndex), 0.5f);
    }

    public void ReloadScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount == 0)
        {
            return;
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadSceneAsync(currentSceneIndex);
    }
}
