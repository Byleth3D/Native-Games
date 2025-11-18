using UnityEngine;

public class MoveTutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ObjectStateManager.Instance.SetAsCollected(this.gameObject);
        GameplayUIManager.Instance.EnablePopup("Move");
        this.gameObject.SetActive(false);
    }
}
