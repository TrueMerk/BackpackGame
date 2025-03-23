using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackpackUI : MonoBehaviour
{
    private Dictionary<int, GameObject> slotMap = new Dictionary<int, GameObject>();
    [SerializeField] private GameObject slotPrefab; 
    
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private Transform toolsContainer;
    [SerializeField] private Transform consumablesContainer;

    [SerializeField] private CanvasGroup canvasGroup; 

    private void Start()
    {
        Backpack.Instance.onInventoryChange.AddListener(UpdateUI);
        HideUI(); 
    }

    private Transform GetContainerForItem(InventoryItem item)
    {
        switch(item.ItemType)
        {
            case ItemType.Weapon:
                return weaponsContainer;
            case ItemType.Tool:
                return toolsContainer;
            case ItemType.Consumable:
                return consumablesContainer;
            default:
                return null;
        }
    }

    private void UpdateUI(InventoryItem item, string action)
    {
        if (action == "added")
        {
            if (!slotMap.ContainsKey(item.ItemID))
            {
                GameObject prefabToUse = item.UiSlotPrefab != null ? item.UiSlotPrefab : slotPrefab;
                Transform parentContainer = GetContainerForItem(item);
                if (parentContainer == null)
                {
                    Debug.LogError("Не найден контейнер для предмета типа: " + item.ItemType);
                    return;
                }
                Debug.Log("Создан слот для предмета с ID: " + item.ItemID);

                GameObject slot = Instantiate(prefabToUse, parentContainer, false);

                SlotHoverRemover hoverRemover = slot.GetComponent<SlotHoverRemover>();
                if (hoverRemover == null)
                {
                    hoverRemover = slot.AddComponent<SlotHoverRemover>();
                }
                int capturedID = item.ItemID;
                hoverRemover.onRelease = () =>
                {
                    Debug.Log("Отпущена кнопка на слоте предмета с ID: " + capturedID);
                    RemoveItemFromBackpack(capturedID);
                };

                slotMap.Add(item.ItemID, slot);
            }
        }
        else if (action == "removed")
        {
            if (slotMap.TryGetValue(item.ItemID, out GameObject slot))
            {
                Destroy(slot);
                slotMap.Remove(item.ItemID);
            }
        }
    }

    private void RemoveItemFromBackpack(int itemID)
    {
        Backpack.Instance.RemoveItem(itemID);
    }

    public void ShowUI()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
