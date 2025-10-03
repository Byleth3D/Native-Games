using System;
using System.Collections.Generic;

public class InventoryManager : Singleton<InventoryManager>
{
    public int ItemCount { get; private set; } = 0;

    public void Collect()
    {
        ItemCount++;
        UIManager.Instance.UpdateItemDisplay(ItemCount);
    }

    public void Deliver()
    {
        ItemCount = 0;
        UIManager.Instance.UpdateItemDisplay(ItemCount);
    }
}
