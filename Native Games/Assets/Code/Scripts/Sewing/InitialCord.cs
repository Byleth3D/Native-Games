using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class InitialCord : Cord, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform cordRectTransform;
    private RectTransform canvasRectTransform;

    private UILineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        cordRectTransform = transform as RectTransform;
        canvasRectTransform = cordRectTransform.GetParentCanvas().transform as RectTransform;

        lineRenderer = GetComponentInChildren<UILineRenderer>();
    }

    public override void Setup(Color color, Sewing sewing)
    {
        base.Setup(color, sewing);

        if (lineRenderer != null)
        {
            lineRenderer.color = color;
        }
    }

    private void HideCord()
    {
        lineRenderer.enabled = false;
        lineRenderer.Points[0] = Vector2.zero;
        lineRenderer.Points[1] = Vector2.zero;
        lineRenderer.SetAllDirty();
        return;
    }

    private void ShowCord()
    {
        Vector2 pointerScreenPoint = InputManager.Instance.GetPointerPosition();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cordRectTransform, pointerScreenPoint, null, out Vector2 pointerCanvasPoint);

        Vector2 offset = cordRectTransform.InverseTransformPoint(lineRenderer.gameObject.transform.position);

        lineRenderer.enabled = true;
        lineRenderer.Points[0] = Vector2.zero;
        lineRenderer.Points[1] = pointerCanvasPoint - offset;

        lineRenderer.SetAllDirty();

        Debug.Log($"Pointer Screen: {pointerScreenPoint} | Local Screen: {pointerCanvasPoint} | Transform Positon {cordRectTransform.transform.position}");
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

        if (lineRenderer.Points.Length < 2)
        {
            return;
        }

        ShowCord();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsConnected)
        {
            return;
        }

        if (sewing.HoveredCord && CordColor == sewing.HoveredCord.CordColor && Direction == sewing.HoveredCord.Direction)
        {
            IsConnected = true;
            sewing.HoveredCord.IsConnected = true;
            sewing.CheckCompletition();
        }
        else
        {
            HideCord();
        }

        sewing.DraggedCord = null;
    }
}