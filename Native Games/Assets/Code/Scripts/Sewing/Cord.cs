using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Cord : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private bool leftCord;
    private RectTransform cordRectTransform;
    private RectTransform canvasRectTransform;

    private Image image;
    private UILineRenderer lineRenderer;

    private bool isDragged;

    public Color CordColor { get; private set; }
    public bool LeftCord => leftCord;
    public bool IsConnected { get; set; }
    public Sewing Sewing { get; set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        cordRectTransform = transform as RectTransform;
        canvasRectTransform = cordRectTransform.GetParentCanvas().transform as RectTransform;

        if (leftCord)
        {
            lineRenderer = GetComponentInChildren<UILineRenderer>();
        }
    }

    public void SetColor(Color color)
    {
        CordColor = color;
        image.color = color;

        if (lineRenderer != null)
        {
            lineRenderer.color = color;
        }
    }

    private void Update()
    {
        if (IsConnected)
        {
            return;
        }

        if (!leftCord)
        {
            Vector2 pointerScreenPoint = InputManager.Instance.GetPointerPosition();
            bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(cordRectTransform, pointerScreenPoint);

            if (isHovered)
            {
                Sewing.HoveredCord = this;
            }
            else
            {
                if (Sewing.HoveredCord == this)
                {
                    Sewing.HoveredCord = null;
                }
            }

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

        if (!isDragged)
        {
            HideCord();
            return;
        }

        ShowCord();
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerScreenPoint, null, out Vector2 local);
        Vector2 local2 = canvasRectTransform.InverseTransformPoint(transform.position);

        lineRenderer.enabled = true;
        lineRenderer.Points[0] = Vector2.zero;
        lineRenderer.Points[1] = local - local2;

        lineRenderer.SetAllDirty();

        Debug.Log($"Pointer Screen: {pointerScreenPoint} | Local Screen: {local} | Transform Positon {cordRectTransform.transform.position}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!leftCord)
        {
            return;
        }

        if (IsConnected)
        {
            return;
        }

        isDragged = true;
        Sewing.DraggedCord = this;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!leftCord)
        {
            return;
        }

        if (Sewing.HoveredCord && CordColor == Sewing.HoveredCord.CordColor)
        {
            IsConnected = true;
            Sewing.HoveredCord.IsConnected = true;
        }

        isDragged = false;
        Sewing.DraggedCord = null;
    }
}