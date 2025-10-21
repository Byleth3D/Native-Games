using PrimeTween;
using System;
using UnityEngine;

public class LoadingScreenManager : Singleton<LoadingScreenManager>
{
    [SerializeField] private GameObject loadingBase;

    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private Ease fadeInEase;

    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private Ease fadeOutEase;

    private CanvasGroup canvasGroup;

    private Tween fadeTween;

    public bool IsEnabled => loadingBase.activeInHierarchy;
    public bool IsVisible { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = loadingBase.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.0f;
        loadingBase.SetActive(false);
    }

    public void EnableLoadingScreen()
    {
        if (IsEnabled || IsVisible)
        {
            return;
        }

        loadingBase.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        fadeTween.Stop();
        fadeTween = Tween.Alpha(canvasGroup, 1f, duration: fadeInDuration, ease: fadeInEase);
        fadeTween.OnComplete(() => IsVisible = true);
    }

    public void EnableLoadingScreen(Action callback)
    {
        if (IsEnabled || IsVisible)
        {
            return;
        }

        loadingBase.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        fadeTween.Stop();
        fadeTween = Tween.Alpha(canvasGroup, 1f, duration: fadeInDuration, ease: fadeInEase);
        fadeTween.OnComplete(() =>
        {
            IsVisible = true;
            callback?.Invoke();
        });
    }

    public void DisableLoadingScreen()
    {
        if (!IsEnabled)
        {
            return;
        }

        fadeTween.Stop();
        fadeTween = Tween.Alpha(canvasGroup, 0.0f, duration: fadeOutDuration, ease: fadeInEase);
        fadeTween.OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            IsVisible = false;
            loadingBase.SetActive(false);
        });
    }

    public void DisableLoadingScreen(Action callback)
    {
        if (!IsEnabled)
        {
            return;
        }

        fadeTween.Stop();
        fadeTween = Tween.Alpha(canvasGroup, 0.0f, duration: fadeOutDuration, ease: fadeInEase);
        fadeTween.OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            IsVisible = false;
            callback?.Invoke();
            loadingBase.SetActive(false);
        });
    }
}
