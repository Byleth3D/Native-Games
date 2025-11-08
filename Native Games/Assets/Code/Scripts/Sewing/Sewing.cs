using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sewing : MonoBehaviour
{
    [SerializeField] private List<Color> cordColors;
    [SerializeField] private List<Cord> cords;
    [Space]
    [SerializeField] private UnityEvent OnComplete;

    private List<Color> availableCordColors;
    private List<Cord> availableHorizontalInitialCords;
    private List<Cord> availableHorizontalFinalCords;
    private List<Cord> availableVerticalInitialCords;
    private List<Cord> availableVerticalFinalCords;

    public InitialCord DraggedCord { get; set; }
    public FinalCord HoveredCord { get; set; }


    private void Start()
    {
        foreach (Cord cord in cords)
        {
            cord.Setup(this);
        }
    }

    public void CheckCompletition()
    {
        if (cords.All(cord => cord.IsConnected))
        {
            OnComplete?.Invoke();
        }
    }
}
