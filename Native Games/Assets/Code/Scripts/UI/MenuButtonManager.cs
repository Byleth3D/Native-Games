using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuButtonManager
{
    [SerializeField] private List<MenuButton> menuButtons;

    private MenuButton FindMenuButton(string menuButtonName)
    {
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

    public void EnableAllMenuButtons()
    {
        foreach (MenuButton menuButton in menuButtons)
        {
            menuButton.button.interactable = false;
        }
    }

    public void DisableAllMenuButtons()
    {
        foreach (MenuButton menuButton in menuButtons)
        {
            menuButton.button.interactable = false;
        }
    }

    public void EnableMenuButton(string menuButtonName)
    {
        if (string.IsNullOrEmpty(menuButtonName))
        {
            return;
        }

        MenuButton menuButton = FindMenuButton(menuButtonName);

        if (menuButton == null)
        {
            return;
        }

        menuButton.button.interactable = true;
    }

    public void DisableMenuButton(string menuButtonName)
    {
        if (string.IsNullOrEmpty(menuButtonName))
        {
            return;
        }

        MenuButton menuButton = FindMenuButton(menuButtonName);

        if (menuButton == null)
        {
            return;
        }

        menuButton.button.interactable = false;
    }
}

[Serializable]
public class MenuButton
{
    public string buttonName;
    public Button button;
}