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

        if (lineRenderer != null)
        {
            lineRenderer.color = color;

            Color transparentImageColor = color;
            transparentImageColor.a = 0.0f;

            image.color = transparentImageColor;
        }
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
        }
        else
        {
            HideCord();
        }

        sewing.DraggedCord = null;
    }
}