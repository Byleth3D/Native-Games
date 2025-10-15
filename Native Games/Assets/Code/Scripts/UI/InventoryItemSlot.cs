using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, ISelectHandler, IUpdateSelectedHandler, IDeselectHandler
{
    [SerializeField] private TextMeshPro itemAmount;
    [SerializeField] private Image itemImage;
    private InventoryItem inventoryItem;

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Selected");

        if (inventoryItem == null)
        {
            return;
        }

        //Pedir para UIManager atualizar as informações do Slot Info
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Deselected");

        if (inventoryItem == null)
        {
            return;
        }

        //Pedir para UIManager remover as informações do Slot Info
    }

    public void Add(int itemAmount)
    {
        this.itemAmount.text = $"{itemAmount}";
    }

    public void Add(InventoryItem inventoryItem, int itemAmount)
    {
        this.inventoryItem = inventoryItem;
        this.itemAmount.text = $"{itemAmount}";
        itemImage.sprite = inventoryItem.itemIcon;
    }

    public void Remove(int itemAmount)
    {
        this.itemAmount.text = $"{itemAmount}";
    }

    public void RemoveAll()
    {
        inventoryItem = null;
        this.itemAmount.text = $"{0}";
        itemImage.sprite = inventoryItem.itemIcon;
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
    }
}
