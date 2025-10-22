
using System.Runtime.CompilerServices;

public class MainMenuManager : LocalSingleton<MainMenuManager>
{
    public InGameMenuManager inGameMenuManager;
    public MenuButtonManager menuButtonManager;

    protected override void Awake()
    {
        base.Awake();
        inGameMenuManager.Setup();
    }

    private void Start()
    {
        CheckIfCanContinue();
    }

    private void Update()
    {
        if (InputManager.Instance.CancelPressed)
        {
            if (!inGameMenuManager.HasOverlappingMenu)
            {
                EnableInGameMenu("Exit");
                return;
            }

            inGameMenuManager.DisableActiveMenu();
        }
    }

    public void NewGame()
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        menuButtonManager.DisableAllMenuButtons();
        SceneLoader.Instance.StartLoading(LoadingType.NewGame);
    }

    public void ContinueGame()
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
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
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
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
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        SceneLoader.Instance.StartLoading("CreditsScene");
    }

    public void ExitGame()
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        GameManager.Instance.ExitGame();
    }
}