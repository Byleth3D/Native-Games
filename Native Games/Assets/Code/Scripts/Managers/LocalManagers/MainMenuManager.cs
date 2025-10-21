
public class MainMenuManager : LocalSingleton<MainMenuManager>
{
    public InGameMenuManager inGameMenuManager;
    public MenuButtonManager menuButtonManager;

    private void Start()
    {
        CheckIfCanContinue();
    }

    public void NewGame()
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        menuButtonManager.DisableAllMenuButtons();
        SceneLoader.Instance.StartLoading(LoadingType.NewGame);
    }

    public void ContinueGame()
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        menuButtonManager.DisableAllMenuButtons();
        SceneLoader.Instance.StartLoading(LoadingType.ContinueGame);
    }

    private void CheckIfCanContinue()
    {
        if (SaveManager.Instance.SaveFiles.Count == 0)
        {
            menuButtonManager.DisableMenuButton("Continue");
        }
        else
        {
            menuButtonManager.EnableMenuButton("Continue");
        }
    }

    public void EnableInGameMenu(string inGameMenu)
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        inGameMenuManager.DisableInGameMenu(inGameMenu);
    }

    public void DisableAllMenuButtons()
    {
        menuButtonManager.DisableAllMenuButtons();
    }

    public void Credits()
    {
        SceneLoader.Instance.StartLoading("CreditsScene");
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}