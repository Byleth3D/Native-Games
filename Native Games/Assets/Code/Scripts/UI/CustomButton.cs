using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            button.OnDeselect(eventData);
            return;
        }

        onClick?.Invoke();
        button.interactable = false;
    }
}
