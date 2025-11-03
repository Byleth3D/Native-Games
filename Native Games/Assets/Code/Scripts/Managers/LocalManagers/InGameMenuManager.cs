using EditorAttributes;
using PrimeTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InGameMenuManager
{
    [SerializeField] private List<InGameMenu> inGameMenus;
    private Tween activeMenuTween;
    private Tween overlappingMenuTween;

    public InGameMenu ActiveMenu { get; private set; }
    public InGameMenu OverlappingMenu { get; private set; }

    public bool HasActiveMenu { get; private set; }
    public bool HasOverlappingMenu { get; private set; }

    public void Setup()
    {
        if (inGameMenus == null)
        {
            return;
        }

        foreach (InGameMenu inGameMenu in inGameMenus)
        {
            if (inGameMenu == null)
            {
                continue;
            }

            inGameMenu.inGameMenuObject.SetActive(false);
            inGameMenu.inGameMenuCanvasGroup = inGameMenu.inGameMenuObject.GetComponent<CanvasGroup>();
            inGameMenu.inGameMenuCanvasGroup.alpha = 0.0f;
            inGameMenu.inGameMenuCanvasGroup.interactable = false;
        }

        ActiveMenu = null;
        OverlappingMenu = null;
    }

    private InGameMenu FindInGameMenu(string inGameMenuName)
    {
        foreach (InGameMenu inGameMenu in inGameMenus)
        {
            if (inGameMenu.inGameMenuName != inGameMenuName)
            {
                continue;
            }

            return inGameMenu;
        }

        return null;
    }

    public void EnableInGameMenu(string inGameMenuName)
    {
        if (string.IsNullOrEmpty(inGameMenuName))
        {
            return;
        }

        InGameMenu inGameMenu = FindInGameMenu(inGameMenuName);

        if (inGameMenu == null)
        {
            return;
        }

        EnableActiveMenu(inGameMenu, inGameMenu.overlaps);
    }

    public void DisableInGameMenu(string inGameMenuName)
    {
        if (string.IsNullOrEmpty(inGameMenuName))
        {
            return;
        }

        InGameMenu inGameMenu = FindInGameMenu(inGameMenuName);

        if (inGameMenu == null)
        {
            return;
        }

        DisableActiveMenu(inGameMenu, inGameMenu.overlaps);
    }

    public bool DisableActiveMenu()
    {
        if (ActiveMenu == null && OverlappingMenu == null)
        {
            return false;
        }

        if (OverlappingMenu != null)
        {
            return DisableActiveMenu(OverlappingMenu, true); ;
        }
        else
        {
            return DisableActiveMenu(ActiveMenu, false);
        }
    }

    public void EnableActiveMenu(InGameMenu inGameMenu, bool overlaps)
    {
        if (inGameMenu == ActiveMenu && inGameMenu == OverlappingMenu)
        {
            return;
        }

        inGameMenu.inGameMenuObject.SetActive(true);
        inGameMenu.inGameMenuCanvasGroup.interactable = true;

        if (overlaps)
        {
            OverlappingMenu = inGameMenu;
            HasOverlappingMenu = true;

            overlappingMenuTween.Stop();
            overlappingMenuTween = Tween.Alpha(inGameMenu.inGameMenuCanvasGroup, 1f, duration: inGameMenu.fadeInDuration, useUnscaledTime: true);
        }
        else
        {
            ActiveMenu = inGameMenu;
            HasActiveMenu = true;

            activeMenuTween.Stop();
            activeMenuTween = Tween.Alpha(inGameMenu.inGameMenuCanvasGroup, 1f, duration: inGameMenu.fadeInDuration, useUnscaledTime: true);
        }

        if (inGameMenu.pauseGame)
        {
            GameManager.Instance.PauseGame();
        }

        InputManager.Instance.DisablePlayerActions();
    }

    public bool DisableActiveMenu(InGameMenu inGameMenu, bool overlaps)
    {
        if (inGameMenu != ActiveMenu && inGameMenu != OverlappingMenu)
        {
            return false;
        }

        inGameMenu.inGameMenuCanvasGroup.interactable = false;

        if (overlaps)
        {
            overlappingMenuTween.Stop();
            overlappingMenuTween = Tween.Alpha(inGameMenu.inGameMenuCanvasGroup, 0.0f, duration: inGameMenu.fadeOutDuration, useUnscaledTime: true);
            overlappingMenuTween.OnComplete(() =>
            {
                OverlappingMenu.inGameMenuObject.SetActive(false);
                OverlappingMenu = null;
                HasOverlappingMenu = false;
            });
        }
        else
        {
            activeMenuTween.Stop();
            activeMenuTween = Tween.Alpha(inGameMenu.inGameMenuCanvasGroup, 0.0f, duration: inGameMenu.fadeOutDuration, useUnscaledTime: true);
            activeMenuTween.OnComplete(() =>
            {
                ActiveMenu.inGameMenuObject.SetActive(false);
                ActiveMenu = null;
                HasActiveMenu = false;
            });
        }

        if (inGameMenu.pauseGame)
        {
            GameManager.Instance.UnpauseGame();
        }

        InputManager.Instance.EnablePlayerActions();
        return true;
    }
}

[Serializable]
public class InGameMenu
{
    public string inGameMenuName;
    public GameObject inGameMenuObject;
    public bool overlaps;
    [DisableField(nameof(overlaps))] public bool pauseGame;
    public float fadeInDuration = 0.25f;
    public float fadeOutDuration = 0.25f;
    [NonSerialized] public CanvasGroup inGameMenuCanvasGroup;
}
