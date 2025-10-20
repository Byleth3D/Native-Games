using UnityEngine;
using UnityEngine.UI;

public abstract class Cord : MonoBehaviour
{
    [SerializeField] protected CordDirection direction = CordDirection.Horizontal;
    protected Image image;
    protected Sewing sewing;

    public Color CordColor { get; private set; }
    public CordDirection Direction => direction;
    public bool IsConnected { get; set; }

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
    }

    public virtual void Setup(Color color, Sewing sewing)
    {
        CordColor = color;
        image.color = color;
        this.sewing = sewing;
    }
}

public enum CordDirection
{
    Vertical, Horizontal
}