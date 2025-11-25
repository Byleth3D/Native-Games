using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingScreen : Singleton<LoadingScreen>
{
    [SerializeField] private GameObject loadingScreen;
    public bool IsVisible { get; private set; } = false;

    public async UniTask SelfEnable()
    {
        IsVisible = true;
        await ScreenFader.Instance.Fade("", FadeType.FadeOut);
        loadingScreen.SetActive(true);
        await UniTask.NextFrame();
        await ScreenFader.Instance.Fade("", FadeType.FadeIn);
        await UniTask.NextFrame();
    }

    public async UniTask SelfDisable()
    {
        await ScreenFader.Instance.Fade("", FadeType.FadeOut);
        IsVisible = false;
        loadingScreen.SetActive(false);
        await UniTask.NextFrame();
        await ScreenFader.Instance.Fade("", FadeType.FadeIn);
        await UniTask.NextFrame();
    }
}