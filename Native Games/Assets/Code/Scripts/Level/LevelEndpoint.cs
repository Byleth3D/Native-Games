using UnityEngine;

public class LevelEndpoint : MonoBehaviour
{
    [SerializeField] private bool triggerOnAwake = false;

    private void Awake()
    {
        if (triggerOnAwake)
        {
            TriggerEndpoint();
        }
    }

    public void TriggerEndpoint()
    {
        SceneLoader.Instance.Load(LoadingType.NextScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnAwake)
        {
            return;
        }

        TriggerEndpoint();
    }
}
