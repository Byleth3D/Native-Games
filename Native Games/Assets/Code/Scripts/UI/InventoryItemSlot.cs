using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemIcon;
    public InventoryItem InventoryItem { get; private set; }
    public int InventoryItemAmount { get; private set; }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Selected");

        if (InventoryItem == null)
        {
            Debug.LogWarning("ItemSlot null");
            return;
        }

        GameplayUIManager.Instance.inventoryMenuManager.ShowInventorySlotInfo(InventoryItem);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Deselected");

        if (InventoryItem == null)
        {
            Debug.LogWarning("ItemSlot null");
            return;
        }

        GameplayUIManager.Instance.inventoryMenuManager.HideInventorySlotInfo();
    }

    public void UpdateAmount(int itemAmount)
    {
        this.itemAmountText.text = $"{itemAmount}";
        InventoryItemAmount = itemAmount;
    }

    public void Add(InventoryItem inventoryItem, int itemAmount)
    {
        InventoryItem = inventoryItem;
        InventoryItemAmount = itemAmount;

        itemAmountText.text = $"{itemAmount}";
        itemIcon.sprite = inventoryItem.itemIcon;

        itemAmountText.gameObject.SetActive(true);
        itemIcon.gameObject.SetActive(true);
    }

    public void Remove()
    {
        InventoryItem = null;
        InventoryItemAmount = 0;

        itemAmountText.text = $"{0}";
        itemIcon.sprite = null;

        itemAmountText.gameObject.SetActive(false);
        itemIcon.gameObject.SetActive(false);
    }
}
