using UnityEngine;

public class CreditsMenuManager : MonoBehaviour
{
    public MenuButtonManager menuButtonManager;

    public void DisableAllMenuButtons()
    {
        menuButtonManager.DisableAllMenuButtons();
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.Load("MainMenu");
    }
}