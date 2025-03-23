using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SlotHoverRemover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovered = false;
    public Action onRelease;  
    private void Update()
    {
        if (isHovered && Input.GetMouseButtonUp(0))
        {
            onRelease?.Invoke();
            isHovered = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
