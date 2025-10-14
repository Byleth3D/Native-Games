using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    [Header("Item Display")]
    [SerializeField] private TextMeshProUGUI itemDisplayText;

    [Header("Menus")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject pauseMenu;
    private GameObject activeMenu;

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

    private void Update()
    {
        if (!GameManager.Instance.Paused)
        {
            if (InputManager.Instance.CancelPressed)
            {
                EnablePauseMenu();
                GameManager.Instance.PauseGame();
                return;
            }

            if (InputManager.Instance.InventoryPressed)
            {
                EnableInventoryMenu();

                return;
            }
        }
        else
        {
            if (InputManager.Instance.CancelPressed)
            {
                DisableActiveMenu();
                GameManager.Instance.UnpauseGame();
            }
        }
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

    public void EnablePauseMenu()
    {
        EnableActiveMenu(pauseMenu);
        GameManager.Instance.PauseGame();
    }

    public void DisablePauseMenu()
    {
        DisableActiveMenu();
    }

    public void EnableInventoryMenu()
    {
        EnableActiveMenu(inventoryMenu);
        GameManager.Instance.PauseGame();
    }

    public void DisableInventoryMenu()
    {
        DisableActiveMenu();
    }

    private void EnableActiveMenu(GameObject menu)
    {
        DisableActiveMenu();
        activeMenu = menu;
        activeMenu.SetActive(true);
    }

    private void DisableActiveMenu()
    {
        if (activeMenu == null)
        {
            return;
        }

        activeMenu.SetActive(false);
        activeMenu = null;
    }
}