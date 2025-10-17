using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayUIManager : Singleton<GameplayUIManager>
{
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
    [SerializeField] private TextMeshProUGUI InventorySlotName;
    [SerializeField] private TextMeshProUGUI InventorySlotDescription;
    [SerializeField] private Image InventorySlotUsageImage;
    [SerializeField] private List<InventoryItemSlot> inventorySlots;

    protected override void Awake()
    {
        base.Awake();
        EnableMovePopUp();
        Invoke(nameof(DisableActivePopup), 5.0f);
        inventoryMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.Instance.Paused)
        {
            if (InputManager.Instance.CancelPressed)
            {
                EnablePauseMenu();
                return;
            }

            if (InputManager.Instance.InventoryPressed)
            {
                EnableInventoryMenu();
            }
        }
        else
        {
            if (InputManager.Instance.CancelPressed)
            {
                DisableActiveMenu();
                return;
            }

            if (InputManager.Instance.InventoryPressed)
            {
                if (activeMenu == pauseMenu)
                {
                    return;
                }

                if (activeMenu == inventoryMenu)
                {
                    DisableInventoryMenu();
                }
            }
        }
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
        GameManager.Instance.UnpauseGame();
    }

    public void EnableInventoryMenu()
    {
        EnableActiveMenu(inventoryMenu);
        GameManager.Instance.PauseGame();
    }

    public void DisableInventoryMenu()
    {
        DisableActiveMenu();
        GameManager.Instance.UnpauseGame();
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
        int inventorySlots = this.inventorySlots.Count;

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
            if (firstEmptyIndex == -1 && this.inventorySlots[i].InventoryItem == null)
            {
                firstEmptyIndex = i;
                continue;
            }

            if (this.inventorySlots[i].InventoryItem == inventoryItem)
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
                this.inventorySlots[firstEmptyIndex].Add(inventoryItem, inventoryItemAmount);
            }
        }
        else
        {
            this.inventorySlots[existingItemIndex].UpdateAmount(inventoryItemAmount);
        }
    }

    public void RemoveInventoryItemFromSlot(InventoryItem inventoryItem, int inventoryItemAmount)
    {
        int index = -1;
        int inventorySlots = this.inventorySlots.Count;

        if (inventoryItem == null || inventoryItemAmount < 0)
        {
            return;
        }

        for (int i = 0; i < inventorySlots; i++)
        {
            if (this.inventorySlots[i].InventoryItem != inventoryItem)
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
            this.inventorySlots[index].Remove();
        }
        else
        {
            this.inventorySlots[index].UpdateAmount(inventoryItemAmount);
        }

        ReorderInventorySlots();
    }

    public void ClearInventorySlots()
    {
        foreach (InventoryItemSlot inventoryItemSlot in inventorySlots)
        {
            inventoryItemSlot.Remove();
        }
    }

    public void ReorderInventorySlots()
    {
        int inventorySlots = this.inventorySlots.Count;

        for (int i = 0; i < inventorySlots; i++)
        {
            if (this.inventorySlots[i].InventoryItem != null || i == inventorySlots - 1)
            {
                continue;
            }

            if (i + 1 == inventorySlots - 1)
            {
                if (this.inventorySlots[i + 1].InventoryItem != null)
                {
                    InventoryItemSlot itemSlot = this.inventorySlots[i + 1];
                    this.inventorySlots[i].Add(itemSlot.InventoryItem, itemSlot.InventoryItemAmount);
                    itemSlot.Remove();
                }
            }
            else
            {
                for (int j = i + 1; j < inventorySlots; j++)
                {
                    if (this.inventorySlots[j].InventoryItem == null)
                    {
                        continue;
                    }

                    InventoryItemSlot itemSlot = this.inventorySlots[j];
                    this.inventorySlots[i].Add(itemSlot.InventoryItem, itemSlot.InventoryItemAmount);
                    itemSlot.Remove();
                    break;
                }
            }
        }
    }

    public void ShowInventorySlotInfo(InventoryItem inventoryItem)
    {
        InventorySlotName.text = inventoryItem.itemName;
        InventorySlotDescription.text = inventoryItem.itemDescription;
        InventorySlotUsageImage.sprite = inventoryItem.itemUsageImage;

        InventorySlotName.gameObject.SetActive(true);
        InventorySlotDescription.gameObject.SetActive(true);
        InventorySlotUsageImage.gameObject.SetActive(true);
    }

    public void HideInventorySlotInfo()
    {
        InventorySlotName.text = "";
        InventorySlotDescription.text = "";
        InventorySlotUsageImage.sprite = null;

        InventorySlotName.gameObject.SetActive(false);
        InventorySlotDescription.gameObject.SetActive(false);
        InventorySlotUsageImage.gameObject.SetActive(false);
    }
    #endregion
}