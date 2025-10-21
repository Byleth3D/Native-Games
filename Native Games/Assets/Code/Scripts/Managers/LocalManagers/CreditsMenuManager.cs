using UnityEngine;

public class CreditsMenuManager : LocalSingleton<CreditsMenuManager>
{
    public MenuButtonManager menuButtonManager;

    public void DisableAllMenuButtons()
    {
        menuButtonManager.DisableAllMenuButtons();
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.StartLoading("MainMenu");
    }

    public void ExitGame()
    {
        DisableAllMenuButtons();
        GameManager.Instance.ExitGame();
    }
}
