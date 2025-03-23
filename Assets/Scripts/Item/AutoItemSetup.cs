using UnityEngine;

[RequireComponent(typeof(ItemObject))] 
public class AutoItemSetup : MonoBehaviour
{
    private void Awake()
    {
        SetupComponents();
    }

    private void SetupComponents()
    {
        if (!TryGetComponent<Rigidbody>(out _))
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;  
        }

        if (!TryGetComponent<Collider>(out _))
        {
            gameObject.AddComponent<BoxCollider>();
        }

        ItemObject item = GetComponent<ItemObject>();
        if (item.itemData == null)
        {
            item.Setup("New Item", Random.Range(1000, 9999), 1.0f, ItemType.Tool);
        }
    }
}
