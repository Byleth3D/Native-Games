using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Cord : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform cordRectTransform;
    [SerializeField] private bool leftCord;

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
        Vector2 lineRendererScreenPointStart = RectTransformUtility.WorldToScreenPoint(null, lineRenderer.transform.position);
        Vector2 pointerScreenPointRaw = InputManager.Instance.GetPointerPosition();
        Vector2 pointerScreenPoint = pointerScreenPointRaw / cordRectTransform.GetParentCanvas().scaleFactor;
        Vector2 lineRendererScreenPointEnd = pointerScreenPoint - lineRendererScreenPointStart;

        lineRenderer.enabled = true;
        lineRenderer.Points[0] = Vector2.zero;
        lineRenderer.Points[1] = lineRendererScreenPointEnd;

        lineRenderer.SetAllDirty();
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