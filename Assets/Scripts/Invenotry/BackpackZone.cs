using UnityEngine;

public class BackpackZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DraggableItem draggable))
        {
            draggable.SetInBackpack(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out DraggableItem draggable))
        {
            draggable.SetInBackpack(false);
        }
    }
}