using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<InventoryItem, int> storedItems = new();
    private List<GameObject> collectedItems = new();

    public bool Collect(GameObject collectableItemObject, InventoryItem inventoryItem)
    {
        bool exists = storedItems.ContainsKey(inventoryItem);
        int amount = 0;

        if (exists)
        {
            int currentAmount = storedItems[inventoryItem];
            int newAmount = currentAmount + inventoryItem.itemAmount;

            if (newAmount > inventoryItem.itemStackLimit)
            {
                return false;
            }

            storedItems[inventoryItem] = newAmount;
            amount = newAmount;
        }
        else
        {
            storedItems.Add(inventoryItem, inventoryItem.itemAmount);
            amount = inventoryItem.itemAmount;
        }

        collectedItems.Add(collectableItemObject);
        GameplayUIManager.Instance.inventoryMenuManager.AddInventoryItemToSlot(inventoryItem, amount);
        return true;
    }

    public bool Deliver(InventoryItem inventoryItem, int requestedAmount)
    {
        if (!storedItems.ContainsKey(inventoryItem))
        {
            return false;
        }

        int currentAmount = storedItems[inventoryItem];

        if (requestedAmount > currentAmount)
        {
            return false;
        }

        int newAmount = currentAmount - requestedAmount;

        if (newAmount == 0)
        {
            storedItems.Remove(inventoryItem);
        }
        else
        {
            storedItems[inventoryItem] = newAmount;
        }

        GameplayUIManager.Instance.inventoryMenuManager.RemoveInventoryItemFromSlot(inventoryItem, newAmount);
        return true;
    }

    public string SaveInventoryItems()
    {
        Dictionary<InventoryItem, int>.KeyCollection inventoryItems = storedItems.Keys;
        int inventoryItemsCount = inventoryItems.Count;
        int index = 0;
        string inventoryItemsInString = "";

        if (inventoryItemsCount > 0)
        {
            foreach (InventoryItem item in inventoryItems)
            {
                if (index == inventoryItemsCount - 1)
                {
                    inventoryItemsInString += $"{item.name}";
                }
                else
                {
                    inventoryItemsInString += $"{item.name}|";
                    index++;
                }
            }
        }

        return inventoryItemsInString;
    }

    public string SaveInventoryItemsAmount()
    {
        Dictionary<InventoryItem, int>.ValueCollection inventoryItems = storedItems.Values;
        int inventoryItemsCount = inventoryItems.Count;
        int index = 0;
        string inventoryItemsAmountInString = "";

        if (inventoryItemsCount > 0)
        {
            foreach (int itemAmount in inventoryItems)
            {
                if (index == inventoryItemsCount - 1)
                {
                    inventoryItemsAmountInString += $"{itemAmount}";
                }
                else
                {
                    inventoryItemsAmountInString += $"{itemAmount}|";
                    index++;
                }
            }
        }

        return inventoryItemsAmountInString;
    }

    public string SaveCollectedItems()
    {
        int collectedItemsCount = collectedItems.Count;
        int index = 0;
        string collectedItemsNames = "";

        if (collectedItemsCount > 0)
        {
            foreach (GameObject item in collectedItems)
            {
                if (index == collectedItemsCount - 1)
                {
                    collectedItemsNames += $"{item.name}";
                }
                else
                {
                    collectedItemsNames += $"{item.name}|";
                    index++;
                }
            }
        }

        return collectedItemsNames;
    }

    public void LoadCollectedItems(string items)
    {
        if (items == "")
        {
            Debug.LogWarning("No Collectable Items Loaded!");

            foreach (GameObject itemObject in collectedItems)
            {
                itemObject.SetActive(true);
                Debug.Log($"{itemObject.name} | {itemObject.activeInHierarchy}");
            }

            return;
        }

        string[] collectedItemsNames = items.Split("|");

        if (collectedItems.Count > 0)
        {
            foreach (GameObject itemObject in collectedItems)
            {
                itemObject.SetActive(true);
                Debug.Log($"{itemObject.name} | {itemObject.activeInHierarchy}");
            }

            collectedItems.Clear();
        }

        foreach (string item in collectedItemsNames)
        {
            GameObject itemObject = GameObject.Find(item);

            if (itemObject != null)
            {
                collectedItems.Add(itemObject);
                itemObject.SetActive(false);
            }
        }
    }

    public void LoadInventory(string items, string itemsAmount)
    {
        if (items == "" || itemsAmount == "")
        {
            Debug.LogWarning("No Inventory Items Were Loaded!");
            return;
        }

        string[] inventoryItems = items.Split("|");
        string[] inventoryItemsAmount = itemsAmount.Split("|");

        if (storedItems.Count > 0)
        {
            storedItems.Clear();
        }

        GameplayUIManager.Instance.inventoryMenuManager.ClearInventorySlots();

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            InventoryItem item = Resources.Load<InventoryItem>("ScriptableObjects/" + inventoryItems[i]);
            int amount = int.Parse(inventoryItemsAmount[i]);

            storedItems.Add(item, amount);
            GameplayUIManager.Instance.inventoryMenuManager.AddInventoryItemToSlot(item, amount);
            Debug.Log($"{item.name} | {storedItems[item]}");
        }
    }
}