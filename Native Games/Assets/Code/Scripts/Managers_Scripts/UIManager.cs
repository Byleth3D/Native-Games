using PrimeTween;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Item Display")]
    [SerializeField] private TextMeshProUGUI itemDisplayText;

    [Header("Popups")]
    [SerializeField] private GameObject movePopup;
    [SerializeField] private GameObject interactPopup;
    [SerializeField] private GameObject interactFailed;
    [SerializeField] private GameObject pushPopup;

    private GameObject activePopup;

    protected override void Awake()
    {
        base.Awake();
        EnableMovePopUp();
        Invoke(nameof(DisableActivePopup), 5.0f);
    }

    public void UpdateItemDisplay(int count)
    {
        itemDisplayText.text = $"{count.ToString()}/3";
    }

    #region Popup Methods
    public void EnableMovePopUp()
    {
        DisableActivePopup();
        SetActivePopup(movePopup);
    }

    public void DisableMovePopUp()
    {
        DisableActivePopup();
    }

    public void EnableInteractPopUp()
    {
        DisableActivePopup();
        SetActivePopup(interactPopup);
    }

    public void DisableInteractPopUp()
    {
        DisableActivePopup();
    }

    public void EnableInteractFailedPopUp()
    {
        DisableActivePopup();
        SetActivePopup(interactFailed);
    }

    public void DisableInteractFailedPopUp()
    {
        DisableActivePopup();
    }

    public void EnablePushPopUp()
    {
        DisableActivePopup();
        SetActivePopup(pushPopup);
    }

    public void DisablePushPopUp()
    {
        DisableActivePopup();
    }

    private void SetActivePopup(GameObject gameObject)
    {
        gameObject.SetActive(true);
        activePopup = gameObject;
    }

    private void DisableActivePopup()
    {
        CancelInvoke(nameof(DisableActivePopup));
        if (activePopup == null) return;

        activePopup.SetActive(false);
        activePopup = null;
    }
    #endregion
}
