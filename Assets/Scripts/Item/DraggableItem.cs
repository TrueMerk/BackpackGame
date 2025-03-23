using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class DraggableItem : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private float fixedZDistance;
    private Rigidbody rb;
    private bool isDroppedByPlayer = false;
    private Coroutine resetCoroutine;
    private float fixedZ;
    private bool isInBackpack = false;
    [SerializeField] private InventoryItem itemData;
    public InventoryItem ItemData => itemData;
    public bool IsInBackpack { get => isInBackpack; set => isInBackpack = value; }

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

     private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        isDragging = true;
        isDroppedByPlayer = false;
         if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
        rb.isKinematic = true;
        fixedZ = transform.position.z;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        rb.isKinematic = false;
        isDroppedByPlayer = true;
        resetCoroutine = StartCoroutine(ResetDroppedFlag());
    }
    private IEnumerator ResetDroppedFlag()
    {
        yield return new WaitForSeconds(0.3f); // ждем 
        isDroppedByPlayer = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); 
            Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 newPos = hitPoint + offset;
                newPos.z = fixedZ;
                rb.MovePosition(newPos);
            }
        }
    }
    public bool IsNotAddable()
    {
        return (isDragging || isInBackpack || !isDroppedByPlayer);
    }
    public void SetInBackpack(bool value)
    {
        isInBackpack = value;
    }
}
