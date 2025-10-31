using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class GameStarter
{
    static GameStarter()
    {
        EditorApplication.playModeStateChanged += SaveActiveEditorScene;
    }

    public static void SaveActiveEditorScene(PlayModeStateChange playeModeState)
    {
        if (playeModeState == PlayModeStateChange.ExitingEditMode)
        {
            string activeSceneName = EditorSceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("ActiveEditorScene", activeSceneName);
            PlayerPrefs.Save();
        }
    }

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
        SceneManager.LoadSceneAsync("GlobalManagers");
#endif
    }
}
