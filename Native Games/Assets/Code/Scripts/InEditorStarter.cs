#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class InEditorStarter
{
    static InEditorStarter()
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
}
#endif
