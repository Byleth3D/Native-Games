using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<InventoryItem, int> storedItens = new();

    public bool Collect(InventoryItem inventoryItem)
    {

        bool exists = storedItens.ContainsKey(inventoryItem);
        Debug.Log($"Item: {inventoryItem} | Exists: {exists}");

        if (exists)
        {
            int currentAmount = storedItens[inventoryItem];
            int newAmount = currentAmount + inventoryItem.itemAmount;

            if (newAmount > inventoryItem.itemStackLimit)
            {
                return false;
            }

            storedItens[inventoryItem] = newAmount;
            UIManager.Instance.UpdateItemDisplay(newAmount);
            Debug.Log($"Item: {inventoryItem.itemName}| Amount: {newAmount}");
        }
        else
        {
            storedItens.Add(inventoryItem, inventoryItem.itemAmount);
            UIManager.Instance.UpdateItemDisplay(inventoryItem.itemAmount);
            Debug.Log($"Item: {inventoryItem.itemName}| Amount: {storedItens[inventoryItem]}");
        }

        return true;
    }

    public bool Deliver(InventoryItem inventoryItem, int requestedAmount)
    {
        if (!storedItens.ContainsKey(inventoryItem))
        {
            return false;
        }

        int currentAmount = storedItens[inventoryItem];

        if (requestedAmount > currentAmount)
        {
            return false;
        }

        int newAmount = currentAmount - requestedAmount;

        if (newAmount == 0)
        {
            storedItens.Remove(inventoryItem);
        }
        else
        {
            storedItens[inventoryItem] = newAmount;
        }

        UIManager.Instance.UpdateItemDisplay(newAmount);
        return true;
    }
}
