using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public InGameMenuManager inGameMenuManager;
    public MenuButtonManager menuButtonManager;

    private void Awake()
    {
        inGameMenuManager.Setup();
    }

    private void Start()
    {
        CheckIfCanContinue();
    }

    private void Update()
    {
        if (InputManager.Instance.UI.CancelPressed)
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
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        menuButtonManager.DisableAllMenuButtons();
        SceneLoader.Instance.Load(LoadingType.NewGame);
    }

    public void ContinueGame()
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        menuButtonManager.DisableAllMenuButtons();
        SceneLoader.Instance.Load(LoadingType.ContinueGame);
    }

    private void CheckIfCanContinue()
    {
        if (SaveManager.Instance.Saves.Count == 0)
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
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
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
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        SceneLoader.Instance.Load("CreditsScene");
    }

    public void ExitGame()
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        GameManager.Instance.SwitchState(GameState.Exiting);
    }
}