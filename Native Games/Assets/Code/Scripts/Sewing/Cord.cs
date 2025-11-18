using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public abstract class Cord : MonoBehaviour
{
    [SerializeField] protected Color color;
    [SerializeField] protected CordDirection direction = CordDirection.Horizontal;
    protected Sewing sewing;
    protected Image image;
    protected UILineRenderer lineRenderer;

    public Color Color => color;
    public CordDirection Direction => direction;
    public bool IsConnected { get; set; }

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        image.color = color;
        lineRenderer = GetComponentInChildren<UILineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.color = color;

            Color transparentImageColor = color;
            transparentImageColor.a = 0.0f;

            image.color = transparentImageColor;
        }
    }

    public virtual void Setup(Sewing sewing)
    {
        this.sewing = sewing;
    }
}

public enum CordDirection
{
    Vertical, Horizontal
}