using UnityEngine;
using UnityEngine.EventSystems;

public class FinalCord : Cord, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public RectTransform snapPoint;
    public RectTransform SnapPoint { get => snapPoint;}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sewing.DraggedCord == null)
        {
            return;
        }

        sewing.HoveredCord = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sewing.HoveredCord && sewing.HoveredCord == this)
        {
            sewing.HoveredCord = null;
        }
    }
}
