using UnityEngine.EventSystems;

public class NewGameButton : CustomButtom
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.StartNewGameLoadingChain();
    }

    public override void OnPointerDown(PointerEventData eventData) { }

    public override void OnPointerUp(PointerEventData eventData) { }
}