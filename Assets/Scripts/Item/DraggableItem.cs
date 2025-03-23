using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableItem : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isDragging = false;
    private Vector3 _offset;
    private float _fixedZDistance;
    private Rigidbody _rb;
    private bool _isDroppedByPlayer = false;
    private Coroutine _resetCoroutine;
    private float _fixedZ;
    private bool _isInBackpack = false;

    [SerializeField] private InventoryItem _itemData;

    public InventoryItem ItemData => _itemData;
    public bool IsInBackpack { get => _isInBackpack; set => _isInBackpack = value; }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        _isDragging = true;
        _isDroppedByPlayer = false;
         if (_resetCoroutine != null)
        {
            StopCoroutine(_resetCoroutine);
            _resetCoroutine = null;
        }
        _rb.isKinematic = true;
        _fixedZ = transform.position.z;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, _fixedZ));
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _offset = transform.position - hitPoint;
        }
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _rb.isKinematic = false;
        _isDroppedByPlayer = true;
        _resetCoroutine = StartCoroutine(ResetDroppedFlag());
    }

    private IEnumerator ResetDroppedFlag()
    {
        yield return new WaitForSeconds(0.3f); // ждем 
        _isDroppedByPlayer = false;
    }

    private void Update()
    {
        if (_isDragging)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); 
            Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, _fixedZ));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 newPos = hitPoint + _offset;
                newPos.z = _fixedZ;
                _rb.MovePosition(newPos);
            }
        }
    }

    public bool IsNotAddable()
    {
        return (_isDragging || _isInBackpack || !_isDroppedByPlayer);
    }

    public void SetInBackpack(bool value)
    {
        _isInBackpack = value;
    }
}
