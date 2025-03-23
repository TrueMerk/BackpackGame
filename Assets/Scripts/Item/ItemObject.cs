using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItem itemData;  

    public void Setup(string name, int id, float weight, ItemType type)
    {
        //itemData = new InventoryItem { itemName = name, itemID = id, weight = weight, itemType = type };
    }
}
