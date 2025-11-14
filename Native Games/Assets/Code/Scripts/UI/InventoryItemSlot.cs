using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemIcon;
    [field: SerializeField] public GameObject Notification { get; private set; }
    public InventoryItem InventoryItem { get; private set; }
    public int InventoryItemAmount { get; private set; }

    public void OnSelect(BaseEventData eventData)
    {
        if (InventoryItem == null)
        {
            return;
        }

        GameplayUIManager.Instance.inventoryMenuManager.ShowInventorySlotInfo(InventoryItem);

        if (Notification.activeInHierarchy)
        {
            Notification.SetActive(false);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (InventoryItem == null)
        {
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
