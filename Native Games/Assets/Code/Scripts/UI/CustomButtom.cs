using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButtom : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public abstract void OnPointerClick(PointerEventData eventData);

    public abstract void OnPointerDown(PointerEventData eventData);

    public abstract void OnPointerUp(PointerEventData eventData);
}
