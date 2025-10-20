using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeManager : Singleton<ScreenFadeManager>
{
    [SerializeField] private bool fadeOnAwake = true;

    [SerializeField] private FadeConfig defaultFadeConfig;
    [SerializeField] private List<FadeConfig> fadeConfigs;
    private Color fadeColor;
    private Color currentColor;

    public float CurrentFadeInDuration { get; set; }
    public float CurrentFadeOutDuration { get; set; }
    public float CurrentCallbackDelay { get; set; }
    public float CurrentFadeDelay { get; set; }

    private FadeType fadeType = FadeType.FadeIn;

    private Action onFadeComplete;

    public bool IsFading { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        fadeType = fadeOnAwake ? FadeType.FadeIn : FadeType.FadeOut;

        CurrentFadeInDuration = defaultFadeConfig.fadeInDuration;
        CurrentFadeOutDuration = defaultFadeConfig.fadeOutDuration;
        CurrentFadeDelay = defaultFadeConfig.fadeDelay;

        CurrentCallbackDelay = defaultFadeConfig.callbackDelay;

        fadeColor = defaultFadeConfig.fadeColor;

        if (fadeOnAwake)
        {
            fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
            currentColor = fadeColor;
            Invoke(nameof(StartFade), CurrentFadeDelay);
        }
    }

    private void OnGUI()
    {

        GUI.color = currentColor;
        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        if (IsFading)
        {
            if (fadeType == FadeType.FadeIn)
            {
                FadeIn();
            }
            else
            {
                FadeOut();
            }
        }
    }

    public void RequestFadeOut()
    {
        if (IsFading || currentColor.a >= 1.0f)
        {
            return;
        }

        fadeColor.a = 0.0f;
        fadeType = FadeType.FadeOut;
        currentColor = fadeColor;

        if (CurrentFadeDelay > 0.0f)
        {
            CancelInvoke(nameof(StartFade));
            Invoke(nameof(StartFade), CurrentFadeDelay);
        }
        else
        {
            CancelInvoke(nameof(StartFade));
            StartFade();
        }
    }

    public void RequestFadeOut(Action callback)
    {
        if (IsFading || currentColor.a >= 1.0f)
        {
            return;
        }

        fadeColor.a = 0.0f;
        fadeType = FadeType.FadeOut;
        currentColor = fadeColor;
        onFadeComplete = callback;

        if (CurrentFadeDelay > 0.0f)
        {
            CancelInvoke(nameof(StartFade));
            Invoke(nameof(StartFade), CurrentFadeDelay);
        }
        else
        {
            CancelInvoke(nameof(StartFade));
            StartFade();
        }
    }

    public void RequestFadeIn()
    {
        if (IsFading || currentColor.a <= 0.0f)
        {
            return;
        }

        fadeColor.a = 1.0f;
        fadeType = FadeType.FadeIn;
        currentColor = fadeColor;

        if (CurrentFadeDelay > 0.0f)
        {
            CancelInvoke(nameof(StartFade));
            Invoke(nameof(StartFade), CurrentFadeDelay);
        }
        else
        {
            CancelInvoke(nameof(StartFade));
            StartFade();
        }
    }

    public void RequestFadeIn(Action callback)
    {
        if (IsFading || currentColor.a <= 0.0f)
        {
            return;
        }

        fadeColor.a = 1.0f;
        fadeType = FadeType.FadeIn;
        currentColor = fadeColor;
        onFadeComplete = callback;

        if (CurrentFadeDelay > 0.0f)
        {
            CancelInvoke(nameof(StartFade));
            Invoke(nameof(StartFade), CurrentFadeDelay);
        }
        else
        {
            CancelInvoke(nameof(StartFade));
            StartFade();
        }
    }

    private void StartFade()
    {
        IsFading = true;
    }

    private void FadeIn()
    {
        if (currentColor.a > 0.0f)
        {
            currentColor.a -= Time.deltaTime / CurrentFadeInDuration;
        }
        else
        {
            IsFading = false;
            fadeType = FadeType.FadeOut;
            Invoke(nameof(OnFadeComplete), CurrentCallbackDelay);
        }
    }

    private void FadeOut()
    {
        if (currentColor.a < 1.0f)
        {
            currentColor.a += Time.deltaTime / CurrentFadeOutDuration;
        }
        else
        {
            IsFading = false;
            fadeType = FadeType.FadeIn;
            Invoke(nameof(OnFadeComplete), CurrentCallbackDelay);
        }
    }

    private void OnFadeComplete()
    {
        onFadeComplete?.Invoke();
        onFadeComplete = null;
    }

    public void ResetConfig()
    {
        CurrentFadeInDuration = defaultFadeConfig.fadeInDuration;
        CurrentFadeOutDuration = defaultFadeConfig.fadeOutDuration;
        CurrentFadeDelay = defaultFadeConfig.fadeDelay;
        CurrentCallbackDelay = defaultFadeConfig.callbackDelay;
        fadeColor = defaultFadeConfig.fadeColor;
    }

    private FadeConfig FindFadeConfig(string fadeConfigName)
    {
        foreach (FadeConfig fadeConfig in fadeConfigs)
        {
            if (fadeConfig.fadeConfigName != fadeConfigName)
            {
                continue;
            }

            return fadeConfig;
        }

        return defaultFadeConfig;
    }

    public void SetConfig(string fadeConfigName)
    {
        if (string.IsNullOrEmpty(fadeConfigName))
        {
            return;
        }

        FadeConfig fadeConfig = FindFadeConfig(fadeConfigName);

        CurrentFadeInDuration = fadeConfig.fadeInDuration;
        CurrentFadeOutDuration = fadeConfig.fadeOutDuration;
        CurrentFadeDelay = fadeConfig.fadeDelay;
        CurrentCallbackDelay = fadeConfig.callbackDelay;
        fadeColor = fadeConfig.fadeColor;
    }
}

public enum FadeType
{
    FadeIn, FadeOut
}

[Serializable]
public class FadeConfig
{
    public string fadeConfigName;
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;
    public float fadeDelay = 0.25f;
    public Color fadeColor = Color.white;
    public float callbackDelay = 0.25f;
}