using UnityEngine;
using System.Collections.Generic;

public class Sewing : MonoBehaviour
{
    [SerializeField] private List<Color> cordColors;
    [SerializeField] private List<Cord> cords;

    private List<Color> availableCordColors;
    private List<Cord> availableInitialCords;
    private List<Cord> availableEndCords;

    public InitialCord DraggedCord { get; set; }
    public FinalCord HoveredCord { get; set; }


    private void Start()
    {
        availableCordColors = new(cordColors);
        availableInitialCords = new List<Cord>();
        availableEndCords = new List<Cord>();


        foreach (Cord cord in cords)
        {
            if (cord is InitialCord)
            {
                availableInitialCords.Add(cord);
            }
            else
            {
                availableEndCords.Add(cord);
            }
        }

        while (cordColors.Count > 0 && availableInitialCords.Count > 0 && availableEndCords.Count > 0)
        {
            int colorIndex = Random.Range(0, availableCordColors.Count);
            Color color = availableCordColors[colorIndex];

            int leftIndex = Random.Range(0, availableInitialCords.Count);
            int rightIndex = Random.Range(0, availableEndCords.Count);

            availableInitialCords[leftIndex].Setup(color, this);
            availableEndCords[rightIndex].Setup(color, this);

            availableCordColors.Remove(color);
            availableInitialCords.RemoveAt(leftIndex);
            availableEndCords.RemoveAt(rightIndex);
        }
    }
}
