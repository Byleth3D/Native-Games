using UnityEngine;

public class Boot : MonoBehaviour
{
    private void Awake()
    {
        string sceneName = SceneManagerWrapper.GetNextSceneName();

        if (PlayerPrefs.HasKey("ActiveEditorScene"))
        {
            string editorSceneName = PlayerPrefs.GetString("ActiveEditorScene");

            if (editorSceneName != "Boot")
            {
                sceneName = editorSceneName;
            }
        }

        SceneLoader.Instance.LoadWithoutLoadingScreen(sceneName);
    }
}
