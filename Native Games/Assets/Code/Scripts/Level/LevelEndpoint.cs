using UnityEngine;
using UnityEngine.Events;

public class LevelEndpoint : MonoBehaviour
{
    public void TriggerEndpoint()
    {
        SceneLoader.Instance.StartLoading(LoadingType.NextScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEndpoint();
    }
}
