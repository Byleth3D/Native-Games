using UnityEngine;

public class CreditsMenuManager : LocalSingleton<CreditsMenuManager>
{
    public MenuButtonManager menuButtonManager;

    public void DisableAllGameButtons()
    {
        menuButtonManager.DisableAllMenuButtons();
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.StartLoading("MainMenu");
    }

    public void ExitGame()
    {
        DisableAllGameButtons();
        GameManager.Instance.ExitGame();
    }
}
