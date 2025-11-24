using UnityEngine;

public class PopupTutorialTrigger : MonoBehaviour
{
    [SerializeField] private string tutorialPopup = "";

    private void OnTriggerEnter(Collider other)
    {
        ObjectStateManager.Instance.SetAsCollected(this.gameObject);
        GameplayUIManager.Instance.EnablePopup(tutorialPopup);
        this.gameObject.SetActive(false);
    }
}
