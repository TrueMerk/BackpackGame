using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackpackUI : MonoBehaviour
{
    private Dictionary<int, GameObject> _slotMap = new Dictionary<int, GameObject>();
    
    [SerializeField] private GameObject _slotPrefab; 
    [SerializeField] private Transform _weaponsContainer;
    [SerializeField] private Transform _toolsContainer;
    [SerializeField] private Transform _consumablesContainer;
    [SerializeField] private CanvasGroup _canvasGroup; 

    private void Start()
    {
        Backpack.Instance.OnInventoryChange.AddListener(UpdateUI);
        HideUI(); 
    }

    private Transform GetContainerForItem(InventoryItem item)
    {
        switch(item.ItemType)
        {
            case ItemType.Weapon:
                return _weaponsContainer;
            case ItemType.Tool:
                return _toolsContainer;
            case ItemType.Consumable:
                return _consumablesContainer;
            default:
                return null;
        }
    }

    private void UpdateUI(InventoryItem item, string action)
    {
        if (action == "added")
        {
            if (!_slotMap.ContainsKey(item.ItemID))
            {
                GameObject prefabToUse = item.UiSlotPrefab != null ? item.UiSlotPrefab : _slotPrefab;
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

                _slotMap.Add(item.ItemID, slot);
            }
        }
        else if (action == "removed")
        {
            if (_slotMap.TryGetValue(item.ItemID, out GameObject slot))
            {
                Destroy(slot);
                _slotMap.Remove(item.ItemID);
            }
        }
    }

    private void RemoveItemFromBackpack(int itemID)
    {
        Backpack.Instance.RemoveItem(itemID);
    }

    public void ShowUI()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void HideUI()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
