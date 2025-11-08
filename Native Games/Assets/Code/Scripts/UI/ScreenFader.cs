using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScreenFader : Singleton<ScreenFader>
{
    [SerializeField] private Fade defaultFade;
    [SerializeField] private List<Fade> fades;
    [SerializeField] private Image target;
    private Canvas canvas;

    private Fade currentFade;

    private Tween fadeTween;

    CancellationToken cancellationToken;

    public bool IsFading { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        canvas = (target.transform as RectTransform).GetParentCanvas();
        cancellationToken = this.GetCancellationTokenOnDestroy();
    }

    public async UniTask Fade(string fadeName, FadeType type, CancellationToken cancellationToken = default, Action callback = null)
    {
        if (cancellationToken == default)
        {
            cancellationToken = this.cancellationToken;
        }

        cancellationToken.ThrowIfCancellationRequested();

        SetConfig(fadeName, type);

        int startDelay = (int)(currentFade.startDelay * 1000);
        await UniTask.Delay(startDelay);

        float endValue = 0.0f;
        float duration = 0.0f;
        SetTweenParameters(type, out endValue, out duration);

        await FadeTween(endValue, duration);

        IsFading = false;
        await DelayCallback(callback);

        ResetConfig();
        canvas.sortingOrder = type == FadeType.FadeIn ? -1 : 99;
    }

    private void SetTweenParameters(FadeType type, out float endValue, out float duration)
    {
        if (type == FadeType.FadeIn)
        {
            endValue = 0.0f;
            duration = currentFade.inDuration;
        }
        else
        {
            endValue = 1f;
            duration = currentFade.outDuration;
            canvas.sortingOrder = 99;
        }
    }

    private async UniTask FadeTween(float endValue, float duration)
    {
        IsFading = true;

        fadeTween = Tween.Alpha
        (target: target, endValue, duration: duration, ease: currentFade.ease);

        await fadeTween;
        await UniTask.NextFrame();
    }

    private async UniTask DelayCallback(Action callback)
    {
        if (callback != null)
        {
            if (currentFade.callbackDelay > 0.0f)
            {
                int callbackDelay = (int)(currentFade.callbackDelay * 1000);
                await UniTask.Delay(callbackDelay);

                callback?.Invoke();
            }
            else
            {
                callback?.Invoke();
            }
        }

        await UniTask.NextFrame();
    }

    public Fade FindFade(string fadeName)
    {
        if (string.IsNullOrEmpty(fadeName))
        {
            return defaultFade;
        }

        return fades.Find(fade => fade.name == fadeName) ?? defaultFade;
    }

    public void SetConfig(string fadeName, FadeType type)
    {
        Fade fade = FindFade(fadeName);
        currentFade = fade;

        Color color = fade.color;
        color.a = type == FadeType.FadeOut ? 0.0f : 1f;

        target.color = color;
    }

    public void ResetConfig()
    {
        currentFade = defaultFade;

        Color color = defaultFade.color;
        color.a = target.color.a;

        target.color = color;
    }

    public void PresetAlphaFor(FadeType type)
    {
        float alpha = type == FadeType.FadeOut ? 0.0f : 1.0f;

        Color color = target.color;
        color.a = alpha;

        target.color = color;
    }
}

public enum FadeType
{
    FadeIn, FadeOut
}

[Serializable]
public class Fade
{
    public string name;
    public Color color = Color.white;
    public Ease ease = Ease.InOutQuad;
    public float inDuration = 1.5f;
    public float outDuration = 1.5f;
    public float startDelay = 0.0f;
    public float callbackDelay = 0.0f;
}