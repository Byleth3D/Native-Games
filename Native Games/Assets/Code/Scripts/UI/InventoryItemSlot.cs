using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemIcon;
    public InventoryItem inventoryItem { get; private set; }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Selected");

        if (inventoryItem == null)
        {
            Debug.LogWarning("ItemSlot null");
            return;
        }

        //Pedir para UIManager atualizar as informações do Slot Info
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("ItemSlot Deselected");

        if (inventoryItem == null)
        {
            Debug.LogWarning("ItemSlot null");
            return;
        }

        itemAmountText.gameObject.SetActive(false);
        itemIcon.gameObject.SetActive(false);
        //Pedir para UIManager remover as informações do Slot Info
    }

    public void UpdateAmount(int itemAmount)
    {
        this.itemAmountText.text = $"{itemAmount}";
    }

    public void Add(InventoryItem inventoryItem, int itemAmount)
    {
        this.inventoryItem = inventoryItem;
        this.itemAmountText.text = $"{itemAmount}";
        itemIcon.sprite = inventoryItem.itemIcon;
        itemAmountText.gameObject.SetActive(true);
        itemIcon.gameObject.SetActive(true);
    }

    public void Remove()
    {
        inventoryItem = null;
        this.itemAmountText.text = $"{0}";
        itemIcon.sprite = null;
    }
}
