using UnityEngine.EventSystems;

public class ContinueLastGameButton : CustomButtom
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.StartLastGameLoadingChain();
    }

    public override void OnPointerDown(PointerEventData eventData) { }

    public override void OnPointerUp(PointerEventData eventData) { }
}
