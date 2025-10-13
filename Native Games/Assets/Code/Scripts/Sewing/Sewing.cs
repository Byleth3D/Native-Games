using UnityEngine;
using System.Collections.Generic;

public class Sewing : MonoBehaviour
{
    [SerializeField] private List<Color> cordColors;
    [SerializeField] private List<Cord> cords;

    private List<Color> availableCordColors;
    private List<Cord> availableLeftCords;
    private List<Cord> availableRightCords;

    private void Start()
    {
        availableCordColors = new(cordColors);
        availableLeftCords = new List<Cord>();
        availableRightCords = new List<Cord>();


        foreach (Cord cord in cords)
        {
            if (cord.LeftCord)
            {
                availableLeftCords.Add(cord);
            }
            else
            {
                availableRightCords.Add(cord);
            }
        }

        while (cordColors.Count > 0 && availableLeftCords.Count > 0 && availableRightCords.Count > 0)
        {
            int colorIndex = Random.Range(0, availableCordColors.Count);
            Color color = availableCordColors[colorIndex];

            int leftIndex = Random.Range(0, availableLeftCords.Count);
            int rightIndex = Random.Range(0, availableRightCords.Count);

            availableLeftCords[leftIndex].SetColor(color);
            availableRightCords[rightIndex].SetColor(color);

            availableCordColors.Remove(color);
            availableLeftCords.RemoveAt(leftIndex);
            availableRightCords.RemoveAt(rightIndex);
        }
    }
}
