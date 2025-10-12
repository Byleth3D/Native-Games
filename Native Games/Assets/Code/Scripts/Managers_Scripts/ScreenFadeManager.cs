using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ScreenFadeManager : Singleton<ScreenFadeManager>
{
    [SerializeField] private bool fadeOnAwake = true;

    [SerializeField] private float defaultFadeInDuration = 1.0f;
    [SerializeField] private float defaultFadeOutDuration = 1.0f;
    [SerializeField] private float defaultFadeDelay = 1.0f;

    public float CurrentFadeInDuration { get; set; }
    public float CurrentFadeOutDuration { get; set; }
    public float CurrentFadeDelay { get; set; }

    [SerializeField] private Color fadeColor = Color.white;
    private Color currentColor;

    [SerializeField] private float defaultCallbackDelay = 1.0f;
    private Action onFadeComplete;
    public float CurrentCallbackDelay { get; set; }

    private FadeType fadeType = FadeType.FadeIn;

    public bool IsFading { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        fadeType = fadeOnAwake ? FadeType.FadeIn : FadeType.FadeOut;

        CurrentFadeInDuration = defaultFadeInDuration;
        CurrentFadeOutDuration = defaultFadeOutDuration;
        CurrentFadeDelay = defaultFadeDelay;

        CurrentCallbackDelay = defaultCallbackDelay;

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
        Invoke(nameof(StartFade), CurrentFadeDelay);
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
        Invoke(nameof(StartFade), CurrentFadeDelay);
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
        Invoke(nameof(StartFade), CurrentFadeDelay);
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
        Invoke(nameof(StartFade), CurrentFadeDelay);
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
        Debug.Log("Calledback");
        onFadeComplete = null;
    }

    public void ResetValues()
    {
        CurrentFadeInDuration = defaultFadeInDuration;
        CurrentFadeOutDuration = defaultFadeOutDuration;
        CurrentFadeDelay = defaultFadeDelay;
    }
}

public enum FadeType
{
    FadeIn, FadeOut
}