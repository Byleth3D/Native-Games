using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PopupManager
{
    [SerializeField] private GameObject popupMenu;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private List<Popup> popups;

    public Popup ActivePopup { get; private set; }
    public bool HasActivePopup { get; private set; }

    public void Setup()
    {
        if (popups == null)
        {
            return;
        }

        popupMenu.SetActive(false);
    }

    private Popup FindPopup(string popupName)
    {
        foreach (Popup popup in popups)
        {
            if (popup.popupName != popupName)
            {
                continue;
            }

            return popup;
        }

        return null;
    }

    public void EnablePopup(string popupName)
    {
        if (string.IsNullOrEmpty(popupName))
        {
            return;
        }

        Popup popup = FindPopup(popupName);

        if (popup == null)
        {
            return;
        }

        EnableActivePopup(popup);
    }

    public void DisablePopup(string popupName)
    {
        if (string.IsNullOrEmpty(popupName))
        {
            return;
        }

        Popup popup = FindPopup(popupName);

        if (popup == null)
        {
            return;
        }

        DisableActivePopup(popup);
    }

    public void EnableActivePopup(Popup popup)
    {
        if (popup == ActivePopup || HasActivePopup)
        {
            return;
        }

        popupMenu.SetActive(true);
        popupText.text = popup.popupMessage;

        HasActivePopup = true;
        ActivePopup = popup;
    }

    public void DisableActivePopup()
    {
        if (!HasActivePopup || ActivePopup == null)
        {
            return;
        }

        DisableActivePopup(ActivePopup);
    }

    public void DisableActivePopup(Popup popup)
    {
        if (popup != ActivePopup || !HasActivePopup)
        {
            return;
        }

        popupMenu.SetActive(false);
        popupText.text = "";

        HasActivePopup = false;
        ActivePopup = null;
    }
}

[Serializable]
public class Popup
{
    public string popupName;
    [TextArea] public string popupMessage;
}
