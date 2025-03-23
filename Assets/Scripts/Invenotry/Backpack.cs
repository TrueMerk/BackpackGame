using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Backpack : MonoBehaviour
{
    private IInventoryNetworkService _networkService;
    private Dictionary<int, GameObject> _storedItems = new Dictionary<int, GameObject>();

    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();  
    [SerializeField] private Transform[] _attachPoints;
    [SerializeField] private BackpackUI _backpackUI; 
    [SerializeField] private Transform _extractionPoint;  // Точка, куда извлекаются предметы
    [SerializeField] private InventoryAnimator _animator;  // Компонент для анимации переходов
    
    public UnityEvent<InventoryItem, string> OnInventoryChange;
    public IReadOnlyList<InventoryItem> Items => _items.AsReadOnly();
    public static Backpack Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        _networkService = new UnityInventoryNetworkService();
    }

    /// <summary>
    /// Перед добавлением проверяем, можно ли добавить предмет
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
        if (_attachPoints == null || _attachPoints.Length <= (int)item.ItemType || _attachPoints[(int)item.ItemType] == null)
        {
            Debug.LogError($"Attach point для {item.ItemType} не найден! Проверь массив attachPoints.");
            return;
        }
        
        if (!_items.Contains(item))
            _items.Add(item);
        
        Transform targetPoint = _attachPoints[(int)item.ItemType];
        draggable.transform.SetParent(targetPoint);
        draggable.gameObject.name = "Item_" + item.ItemID;
        _storedItems[item.ItemID] = draggable.gameObject;
        
        StartCoroutine(_animator.AnimateSnapToPosition(draggable.transform, Vector3.zero, Quaternion.identity, 0.5f));
        
        OnInventoryChange?.Invoke(item, "added");
        StartCoroutine(_networkService.SendInventoryEvent(item, "added"));
    }

    /// <summary>
    /// Извлекает предмет из инвентаря по itemID.
    /// Удаляет предмет из списка, извлекает объект из словаря,
    /// запускает анимацию извлечения до extractionPoint и отправляет событие.
    /// </summary>
    public void RemoveItem(int itemID)
    {
        InventoryItem target = _items.Find(x => x.ItemID == itemID);
        if (target != null)
        {
            _items.Remove(target);
            if (_storedItems.TryGetValue(itemID, out GameObject obj))
            {
                _storedItems.Remove(itemID);
                obj.transform.SetParent(null);
                StartCoroutine(_animator.AnimateUnsnap(obj.transform, _extractionPoint.position, Quaternion.identity, 0.5f));
            }
            
            OnInventoryChange?.Invoke(target, "removed");
            StartCoroutine(_networkService.SendInventoryEvent(target, "removed"));
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
