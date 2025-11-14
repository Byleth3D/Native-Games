using Cysharp.Threading.Tasks;
using EditorAttributes;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PopupManager
{
    [SerializeField] private GameObject popupMenu;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private List<Popup> popups;

    [SerializeField] private GameObject itemPopupMenu;
    [SerializeField] private Image itemPopupImage;
    [SerializeField] private TextMeshProUGUI itemPopupText;
    [SerializeField] private List<ItemPopup> itemPopups;

    private CountdownTimer popupTimer;

    private CancellationTokenSource cancellationTokenSource;

    public Popup ActivePopup { get; private set; }
    public GameObject ActivePopupMenu { get; private set; }
    public TextMeshProUGUI ActivePopupText { get; private set; }
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

        foreach (ItemPopup popup in itemPopups)
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

        if (popup.GetType() == typeof(ItemPopup))
        {
            ActivePopupMenu = itemPopupMenu;
            ActivePopupText = itemPopupText;
            itemPopupImage.sprite = (popup as ItemPopup).icon;
        }
        else
        {
            ActivePopupMenu = popupMenu;
            ActivePopupText = popupText;
        }

        ActivePopupMenu.SetActive(true);

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

        ActivePopupText.text = newPopupText;

        HasActivePopup = true;
        ActivePopup = popup;

        if (popup.isTimeBased)
        {
            popupTimer = new CountdownTimer(popup.duration);
            popupTimer.StartAndQueue();
            popupTimer.OnTimerExpired += DisableActivePopup;

            cancellationTokenSource = new CancellationTokenSource();
            WaitForTimer(cancellationTokenSource.Token).Forget();
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

        ActivePopupMenu.SetActive(false);
        ActivePopupText.text = "";
        itemPopupImage.sprite = null;

        HasActivePopup = false;
        ActivePopup = null;
        ActivePopupMenu = null;
        ActivePopupText = null;

        if (popup.isTimeBased)
        {
            if (popupTimer.IsRunning || popupTimer.CurrentTime > 0.0f)
            {
                popupTimer.StopAndQueue();
                cancellationTokenSource?.Cancel();
            }
        }
    }

    public async UniTaskVoid WaitForTimer(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (CutsceneManager.Instance.CurrentCutscene != null)
        {
            popupTimer.StopAndQueue();

            await UniTask.WaitUntil(() => CutsceneManager.Instance.CurrentCutscene == null,
                cancellationToken: cancellationToken);

            popupTimer.RestartAndQueue();
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

[Serializable]
public class ItemPopup : Popup
{
    public Sprite icon;
}