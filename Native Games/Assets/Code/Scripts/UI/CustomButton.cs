using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent onClick;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            button.OnDeselect(eventData);
            return;
        }

        onClick?.Invoke();
        button.interactable = false;
    }
}
