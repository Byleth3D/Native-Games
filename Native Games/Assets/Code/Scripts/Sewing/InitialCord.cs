using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class InitialCord : Cord, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private GameObject indicator;
    private RectTransform cordRectTransform;
    //private RectTransform canvasRectTransform;

    protected override void Awake()
    {
        base.Awake();

        cordRectTransform = transform as RectTransform;
        //canvasRectTransform = cordRectTransform.GetParentCanvas().transform as RectTransform;
    }

    private void HideCord()
    {
        lineRenderer.Points[2] = lineRenderer.Points[1];
        lineRenderer.SetAllDirty();
        return;
    }

    private void ShowCord()
    {
        Vector2 pointerScreenPoint = InputManager.Instance.UI.PointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cordRectTransform, pointerScreenPoint, null, out Vector2 pointerCanvasPoint);

        Vector2 offset = cordRectTransform.InverseTransformPoint(lineRenderer.gameObject.transform.position);

        lineRenderer.enabled = true;

        lineRenderer.Points[2] = pointerCanvasPoint - offset;

        lineRenderer.SetAllDirty();
        //Debug.Log($"Pointer Screen: {pointerScreenPoint} | Local Screen: {pointerCanvasPoint} | Transform Positon {cordRectTransform.transform.position}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsConnected)
        {
            return;
        }

        sewing.DraggedCord = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsConnected)
        {
            return;
        }

        if (lineRenderer == null)
        {
            return;
        }

        if (lineRenderer.Points.Length < 3)
        {
            return;
        }

        ShowCord();
        indicator.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsConnected)
        {
            return;
        }

        Color32 thisCordColor = color;
        Color32 otherCordColor = sewing.HoveredCord ? sewing.HoveredCord.Color : Color.black;

        if (sewing.HoveredCord && thisCordColor.Equals(otherCordColor)
            && Direction == sewing.HoveredCord.Direction)
        {
            IsConnected = true;
            sewing.HoveredCord.IsConnected = true;
            sewing.CheckCompletition();

            indicator.SetActive(false);

            SnapCord();
        }
        else
        {
            HideCord();
            indicator.SetActive(true);
        }

        sewing.DraggedCord = null;
    }

    private void SnapCord()
    {
        RectTransform rectTransform = sewing.HoveredCord.SnapPoint;
        Vector3 snapWorldPosition = rectTransform.position;

        Vector2 snapScreenPoint = RectTransformUtility.WorldToScreenPoint(null, snapWorldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRenderer.rectTransform,
            snapScreenPoint,
            null,
            out Vector2 snapLocalPoint
        );

        lineRenderer.Points[2] = snapLocalPoint;
        lineRenderer.SetAllDirty();
    }
}