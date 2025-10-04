using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ScreenFadeManager : Singleton<ScreenFadeManager>
{
    [SerializeField] private bool fadeOnAwake = true;

    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private float fadeDelay = 1.0f;

    [SerializeField] private Color fadeColor = Color.black;
    private Color currentColor;

    [SerializeField] private float callbackDelay = 1.0f;
    private Action onFadeComplete;

    private FadeType fadeType = FadeType.FadeIn;

    private bool isFading = false;

    protected override void Awake()
    {
        base.Awake();

        fadeType = fadeOnAwake ? FadeType.FadeIn : FadeType.FadeOut;

        if (fadeOnAwake)
        {
            fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
            currentColor = fadeColor;
            Invoke(nameof(StartFade), fadeDelay);
        }
    }

    private void OnGUI()
    {

        GUI.color = currentColor;
        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        if (isFading)
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
        if (isFading)
        {
            return;
        }

        fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
        currentColor = fadeColor;
        Invoke(nameof(StartFade), fadeDelay);
    }

    public void RequestFadeOut(float newFadeDelay)
    {
        if (isFading)
        {
            return;
        }

        fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
        currentColor = fadeColor;
        Invoke(nameof(StartFade), newFadeDelay);
    }

    public void RequestFadeOut(Action callback)
    {
        if (isFading)
        {
            return;
        }

        fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
        currentColor = fadeColor;
        Invoke(nameof(StartFade), fadeDelay);
    }

    public void RequestFadeOut(Action callback, float newFadeDelay)
    {
        if (isFading)
        {
            return;
        }

        fadeColor.a = fadeType == FadeType.FadeIn ? 1.0f : 0.0f;
        currentColor = fadeColor;
        Invoke(nameof(StartFade), newFadeDelay);
    }

    private void StartFade()
    {
        isFading = true;
    }

    private void FadeIn()
    {
        if (currentColor.a > 0.0f)
        {
            currentColor.a -= Time.deltaTime / fadeInDuration;
        }
        else
        {
            isFading = false;
            fadeType = FadeType.FadeOut;
            Invoke(nameof(OnFadeComplete), callbackDelay);
        }
    }

    private void FadeOut()
    {
        if (currentColor.a < 1.0f)
        {
            currentColor.a += Time.deltaTime / fadeOutDuration;
        }
        else
        {
            isFading = false;
            fadeType = FadeType.FadeIn;
            Invoke(nameof(OnFadeComplete), callbackDelay);
        }
    }

    private void OnFadeComplete()
    {
        onFadeComplete?.Invoke();
        onFadeComplete = null;
    }
}

public enum FadeType
{
    FadeIn, FadeOut
}
