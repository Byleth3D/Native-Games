using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cord : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private bool leftCord;
    private bool isDragged;
    private Image image;
    private RectTransform canvasRectTransform;

    private bool isConnected;

    public Color CordCollor { get; private set; }
    public bool LeftCord => leftCord;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasRectTransform = GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
    }

    public void SetColor(Color color)
    {
        CordCollor = color;
        image.color = color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
        Debug.Log("Dragging");
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragged = false;
        Debug.Log("Not Dragging");
    }
}
