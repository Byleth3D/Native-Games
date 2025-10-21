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

    }

    public void ExitGame()
    {
        DisableAllGameButtons();
        GameManager.Instance.ExitGame();
    }
}
