using UnityEngine;

public static class GameStarter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SetupGame()
    {
#if !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.Android
    || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 240;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
        }
#else
        if (SceneManagerWrapper.GetActiveSceneName().Equals("Boot"))
        {
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Boot");
#endif
    }
}
