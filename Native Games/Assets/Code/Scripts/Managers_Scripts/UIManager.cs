using System.Collections.Generic;
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

    [Header("Inventory")]
    [SerializeField] private List<InventoryItemSlot> inventoryItemSlots;

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

    #region InventoryDisplay Methods
    public void AddInventoryItemToSlot(InventoryItem inventoryItem, int inventoryItemAmount)
    {
        int inventorySlots = inventoryItemSlots.Count;

        if (inventorySlots == 0)
        {
            Debug.LogError("No Reference to ItemSlots");
            return;
        }

        if (inventoryItem == null || inventoryItemAmount <= 0)
        {
            return;
        }

        int firstEmptyIndex = -1;
        int existingItemIndex = -1;

        for (int i = 0; i < inventorySlots; i++)
        {
            if (firstEmptyIndex == -1 && inventoryItemSlots[i].inventoryItem == null)
            {
                firstEmptyIndex = i;
                continue;
            }

            if (inventoryItemSlots[i].inventoryItem == inventoryItem)
            {
                existingItemIndex = i;
            }
        }

        bool itemExistsInSlots = existingItemIndex != -1;
        bool listHasEmptySlot = firstEmptyIndex != -1;

        if (!itemExistsInSlots)
        {
            if (listHasEmptySlot)
            {
                inventoryItemSlots[firstEmptyIndex].Add(inventoryItem, inventoryItemAmount);
            }
        }
        else
        {
            inventoryItemSlots[existingItemIndex].UpdateAmount(inventoryItemAmount);
        }
    }

    public void RemoveInventoryItemFromSlot(InventoryItem inventoryItem, int inventoryItemAmount)
    {
        int index = -1;
        int inventorySlots = inventoryItemSlots.Count;

        for (int i = 0; i < inventorySlots; i++)
        {
            if (inventoryItemSlots[i].inventoryItem != inventoryItem)
            {
                continue;
            }

            index = i;
        }

        if (index == -1)
        {
            return;
        }

        if (inventoryItemAmount == 0)
        {
            inventoryItemSlots[index].Remove();
        }
        else
        {
            inventoryItemSlots[index].UpdateAmount(inventoryItemAmount);
        }
    }

    public void ClearInventorySlots()
    {
        foreach (InventoryItemSlot inventoryItemSlot in inventoryItemSlots)
        {
            inventoryItemSlot.Remove();
        }
    }
    #endregion
}