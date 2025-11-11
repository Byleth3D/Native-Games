using EditorAttributes;
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

    private CountdownTimer popupTimer;

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
            if (popup.name != popupName)
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

        string newPopupText = "";

        if (popup.isPlataformSpecific)
        {
            newPopupText = Application.platform == RuntimePlatform.Android ?
            popup.mobileMessage : popup.pcMessage;
        }
        else
        {
            newPopupText = popup.defaultMessage;
        }

        popupText.text = newPopupText;

        HasActivePopup = true;
        ActivePopup = popup;

        if (popup.isTimeBased)
        {
            popupTimer = new CountdownTimer(popup.duration);
            popupTimer.StartAndQueue();
            popupTimer.OnTimerExpired += DisableActivePopup;
        }
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

        if (popup.isTimeBased)
        {
            if (popupTimer.IsRunning || popupTimer.CurrentTime > 0.0f)
            {
                popupTimer.StopAndQueue();
            }
        }
    }
}

[Serializable]
public class Popup
{
    public string name;
    public bool isTimeBased;
    [EnableField(nameof(isTimeBased))] public float duration;
    public bool isPlataformSpecific;
    [DisableField(nameof(isPlataformSpecific)), TextArea] public string defaultMessage;
    [EnableField(nameof(isPlataformSpecific)), TextArea] public string pcMessage;
    [EnableField(nameof(isPlataformSpecific)), TextArea] public string mobileMessage;
}
