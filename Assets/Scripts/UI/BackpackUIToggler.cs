using UnityEngine;
using UnityEngine.EventSystems;

public class BackpackUIToggler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private BackpackUI backpackUI;

    public void OnPointerDown(PointerEventData eventData)
    {
        backpackUI.ShowUI();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        backpackUI.HideUI();
    }
}
