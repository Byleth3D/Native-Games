using UnityEngine;
using UnityEngine.UI;

public abstract class Cord : MonoBehaviour
{
    [SerializeField] protected Color color;
    [SerializeField] protected CordDirection direction = CordDirection.Horizontal;
    protected Sewing sewing;
    protected Image image;

    public Color Color => color;
    public CordDirection Direction => direction;
    public bool IsConnected { get; set; }

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        image.color = color;
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