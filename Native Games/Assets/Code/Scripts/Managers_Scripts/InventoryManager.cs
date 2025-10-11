using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<InventoryItem, int> storedItems = new();

    public bool Collect(InventoryItem inventoryItem)
    {
        bool exists = storedItems.ContainsKey(inventoryItem);

        if (exists)
        {
            int currentAmount = storedItems[inventoryItem];
            int newAmount = currentAmount + inventoryItem.itemAmount;

            if (newAmount > inventoryItem.itemStackLimit)
            {
                return false;
            }

            storedItems[inventoryItem] = newAmount;
            UIManager.Instance.UpdateItemDisplay(newAmount);
        }
        else
        {
            storedItems.Add(inventoryItem, inventoryItem.itemAmount);
            UIManager.Instance.UpdateItemDisplay(inventoryItem.itemAmount);
        }

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

        UIManager.Instance.UpdateItemDisplay(newAmount);
        return true;
    }

    public void LoadInventory(string items, string itemsAmount)
    {
        string[] inventoryItems = items.Split("|");
        string[] inventoryItemsAmount = itemsAmount.Split("|");

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            InventoryItem item = Resources.Load<InventoryItem>("ScriptableObjects/" + inventoryItems[i]);
            int amount = int.Parse(inventoryItemsAmount[i]);

            storedItems.Add(item, amount);
            Debug.Log($"{item.name} | {storedItems[item]}");

            if (item.name == "CottonCord")
            {
                UIManager.Instance.UpdateItemDisplay(storedItems[item]);
            }
        }
    }
}
