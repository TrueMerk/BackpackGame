using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Backpack : MonoBehaviour
{
    private IInventoryNetworkService networkService;

    private Dictionary<int, GameObject> storedItems = new Dictionary<int, GameObject>();
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();  
    
    [SerializeField] private Transform[] attachPoints;
    [SerializeField] private BackpackUI backpackUI; 
    [SerializeField] private Transform extractionPoint;  // Точка, куда извлекаются предметы
    [SerializeField] private InventoryAnimator animator;  // Компонент для анимации переходов

    public UnityEvent<InventoryItem, string> onInventoryChange;
    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();

    public static Backpack Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        networkService = new UnityInventoryNetworkService();
    }

    /// <summary>
    /// Пример обработки триггера – если предмет входит в зону рюкзака, добавляем его.
    /// Перед добавлением проверяем, можно ли его добавить 
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered by {other.name}");
        if (other.TryGetComponent(out DraggableItem draggable))
        {
            if (draggable.IsNotAddable())
                return;
            other.enabled = false;
            draggable.IsInBackpack = true;
            AddItem(draggable.ItemData, draggable);
        }
    }

    /// <summary>
    /// Добавляет предмет в инвентарь.
    /// Проверяет наличие attach point для типа предмета,
    /// добавляет предмет в список, репарентит объект к нужной точке,
    /// запускает анимацию и отправляет сетевое событие.
    /// </summary>
    public void AddItem(InventoryItem item, DraggableItem draggable)
    {
        if (attachPoints == null || attachPoints.Length <= (int)item.ItemType || attachPoints[(int)item.ItemType] == null)
        {
            Debug.LogError($"Attach point для {item.ItemType} не найден! Проверь массив attachPoints.");
            return;
        }
        
        if (!items.Contains(item))
            items.Add(item);
        
        Transform targetPoint = attachPoints[(int)item.ItemType];
        draggable.transform.SetParent(targetPoint);
        draggable.gameObject.name = "Item_" + item.ItemID;
        storedItems[item.ItemID] = draggable.gameObject;
        
        StartCoroutine(animator.AnimateSnapToPosition(draggable.transform, Vector3.zero, Quaternion.identity, 0.5f));
        
        onInventoryChange?.Invoke(item, "added");
        StartCoroutine(networkService.SendInventoryEvent(item, "added"));
    }

    /// <summary>
    /// Извлекает предмет из инвентаря по itemID.
    /// Удаляет предмет из списка, извлекает объект из словаря,
    /// запускает анимацию извлечения до extractionPoint и отправляет событие.
    /// </summary>
    public void RemoveItem(int itemID)
    {
        InventoryItem target = items.Find(x => x.ItemID == itemID);
        if (target != null)
        {
            items.Remove(target);
            if (storedItems.TryGetValue(itemID, out GameObject obj))
            {
                storedItems.Remove(itemID);
                obj.transform.SetParent(null);
                StartCoroutine(animator.AnimateUnsnap(obj.transform, extractionPoint.position, Quaternion.identity, 0.5f));
            }
            
            onInventoryChange?.Invoke(target, "removed");
            StartCoroutine(networkService.SendInventoryEvent(target, "removed"));
            DraggableItem draggable = obj.GetComponent<DraggableItem>();
            if(draggable != null)
            {
                draggable.SetInBackpack(false);
            }
        }
        else
        {
            Debug.LogWarning("Предмет с ID " + itemID + " не найден в рюкзаке!");
        }
    }

    
}
