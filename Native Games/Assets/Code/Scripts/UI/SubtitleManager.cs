using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.InputSystem;

public class SubtitleManager : LocalSingleton<SubtitleManager>
{
    [SerializeField] private TextMeshProUGUI subtitleText;

    private Subtitle currentSubtitle;
    private List<SubtitleLine> currentSubtitleLines;
    private int currentSubtitleLineIndex = -1;

    private AudioSource currentAudioSource;

    private bool subtitleOn;

    private CancellationTokenSource cancellationTokenSource;

    protected override void Awake()
    {
        base.Awake();
    }

    public void TurnOnSubtitle(Subtitle subtitle, AudioSource audioSource)
    {
        if (subtitle == null || subtitle.lines == null)
        {
            return;
        }

        TurnOffSubtitle();

        currentSubtitle = subtitle;
        currentSubtitleLines = subtitle.lines;
        currentSubtitleLineIndex = 0;

        currentAudioSource = audioSource;

        subtitleOn = true;
        cancellationTokenSource = new CancellationTokenSource();
        DisplaySubtitle(cancellationTokenSource.Token).Forget();
    }

    public void TurnOffSubtitle()
    {
        if (!subtitleOn || currentSubtitle == null)
        {
            return;
        }

        cancellationTokenSource.Cancel();

        currentSubtitle = null;
        currentSubtitleLines = null;
        currentSubtitleLineIndex = -1;

        currentAudioSource = null;

        subtitleText.text = "";

        subtitleOn = false;
    }

    private async UniTaskVoid DisplaySubtitle(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int lineCount = currentSubtitle.lines.Count;

        while (currentSubtitleLineIndex < lineCount)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                TurnOffSubtitle();
            }

            SubtitleLine subtitleLine = currentSubtitleLines[currentSubtitleLineIndex];

            await UniTask.WaitWhile(() =>
            currentAudioSource.time < subtitleLine.StartTimeInSeconds,
            cancellationToken: cancellationToken, cancelImmediately: true);

            if (currentAudioSource.time < subtitleLine.EndTimeInSeconds)
            {
                subtitleText.text = subtitleLine.text;
            }
            else
            {
                currentSubtitleLineIndex++;
            }
        }

        TurnOffSubtitle();
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}