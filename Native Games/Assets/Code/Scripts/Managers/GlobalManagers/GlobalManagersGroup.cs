using UnityEngine;

public class GlobalManagersGroup : MonoBehaviour
{
    private void Update()
    {
        int childCount = transform.childCount;

        if (childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
