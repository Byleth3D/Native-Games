using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GlobalManagersGroup : MonoBehaviour
{
    private List<GameObject> managers = new();

    private void Awake()
    {
        int childCount = transform.childCount;
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
    }
}
