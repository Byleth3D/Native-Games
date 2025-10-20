using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

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
        availableCordColors = new(cordColors);
        availableHorizontalInitialCords = new List<Cord>();
        availableHorizontalFinalCords = new List<Cord>();
        availableVerticalInitialCords = new List<Cord>();
        availableVerticalFinalCords = new List<Cord>();


        foreach (Cord cord in cords)
        {
            if (cord is InitialCord)
            {
                if (cord.Direction == CordDirection.Horizontal)
                {
                    availableHorizontalInitialCords.Add(cord);
                }
                else
                {
                    availableVerticalInitialCords.Add(cord);
                }
            }
            else
            {
                if (cord.Direction == CordDirection.Horizontal)
                {
                    availableHorizontalFinalCords.Add(cord);
                }
                else
                {
                    availableVerticalFinalCords.Add(cord);
                }
            }
        }

        while (cordColors.Count > 0 && availableHorizontalInitialCords.Count > 0 && availableHorizontalFinalCords.Count > 0)
        {
            int colorIndex = Random.Range(0, availableCordColors.Count);
            Color color = availableCordColors[colorIndex];

            int initialHorizontalIndex = Random.Range(0, availableHorizontalInitialCords.Count);
            int finalHorizontalIndex = Random.Range(0, availableHorizontalFinalCords.Count);

            int initialVerticalIndex = Random.Range(0, availableVerticalInitialCords.Count);
            int finalVerticalIndex = Random.Range(0, availableVerticalFinalCords.Count);

            availableHorizontalInitialCords[initialHorizontalIndex].Setup(color, this);
            availableHorizontalFinalCords[finalHorizontalIndex].Setup(color, this);

            availableVerticalInitialCords[initialVerticalIndex].Setup(color, this);
            availableVerticalFinalCords[finalVerticalIndex].Setup(color, this);

            availableCordColors.Remove(color);

            availableHorizontalInitialCords.RemoveAt(initialHorizontalIndex);
            availableHorizontalFinalCords.RemoveAt(finalHorizontalIndex);

            availableVerticalInitialCords.RemoveAt(initialVerticalIndex);
            availableVerticalFinalCords.RemoveAt(finalVerticalIndex);
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
