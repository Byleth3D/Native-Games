using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MainMenuManager : LocalSingleton<MainMenuManager>
{
    public InGameMenuManager inGameMenuManager;
    public List<MenuButton> menuButtons;

    private void Start()
    {
        CheckIfCanContinue();
    }

    public void NewGame()
    {
        DisableMenuButtons();
        SceneLoader.Instance.StartNewGameLoadingChain();
    }

    public void ContinueGame()
    {
        DisableMenuButtons();
        SceneLoader.Instance.StartLastGameLoadingChain();
    }

    private void CheckIfCanContinue()
    {
        MenuButton continueButton = FindMenuButton("Continue");

        if (continueButton == null)
        {
            return;
        }

        if (SaveManager.Instance.SaveFiles.Count == 0)
        {
            continueButton.button.interactable = false;
        }
        else
        {
            continueButton.button.interactable = true;
        }
    }

    private MenuButton FindMenuButton(string menuButtonName)
    {
        if (string.IsNullOrEmpty(menuButtonName))
        {
            return null;
        }

        foreach (MenuButton menuButton in menuButtons)
        {
            if (menuButton.buttonName != menuButtonName)
            {
                continue;
            }

            return menuButton;
        }

        return null;
    }

    public void DisableMenuButtons()
    {
        foreach (MenuButton menuButton in menuButtons)
        {
            menuButton.button.interactable = false;
        }
    }

    public void EnableInGameMenu(string inGameMenu)
    {
        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        inGameMenuManager.DisableInGameMenu(inGameMenu);
    }
}


[Serializable]
public class MenuButton
{
    public string buttonName;
    public Button button;
}