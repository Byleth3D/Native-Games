using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryMenuManager
{
    [SerializeField] private TextMeshProUGUI InventorySlotName;
    [SerializeField] private TextMeshProUGUI InventorySlotDescription;
    [SerializeField] private Image InventorySlotUsageImage;
    [SerializeField] private List<InventoryItemSlot> inventorySlots;
    [field: SerializeField] public GameObject Notification { get; private set; }

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

        Notification.SetActive(true);
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

    public void Setup()
    {
        GameplayUIManager.Instance.inventoryMenuManager.ClearInventorySlots();

        Dictionary<InventoryItem, int> storeItems = InventoryManager.Instance.GetStoredItems();

        if (storeItems == null || storeItems.Count == 0)
        {
            return;
        }

        foreach (InventoryItem item in storeItems.Keys)
        {
            AddInventoryItemToSlot(item, storeItems[item]);
        }
    }
}