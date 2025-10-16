using EditorAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;
    public Sprite itemUsageImage;

    public bool isStackable = false;
    [EnableField(nameof(isStackable))] public int itemAmount = 1;
    [EnableField(nameof(isStackable))] public int itemStackLimit = 1;
}
