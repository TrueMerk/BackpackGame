using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private int itemID;
    [SerializeField] private string itemName;
    [SerializeField] private float weight;
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject uiSlotPrefab;

    public int ItemID => itemID;
    public string ItemName => itemName;
    public float Weight => weight;
    public ItemType ItemType => itemType;
    public GameObject Prefab => prefab;
    public GameObject UiSlotPrefab => uiSlotPrefab;
}

public enum ItemType
{
    Weapon,
    Tool,
    Consumable
}
