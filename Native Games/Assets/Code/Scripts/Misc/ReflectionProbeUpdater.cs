using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ReflectionProbeUpdater : MonoBehaviour
{
    [SerializeField] private ReflectionProbe reflectionProbe;
    private int reflectionProbeId = -1;
    private CancellationTokenSource cancellationTokenSource;

    private void OnEnable()
    {
        reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        reflectionProbe.RenderProbe();
    }

    private void OnDisable()
    {
        cancellationTokenSource?.Cancel();

        if (Application.isPlaying)
        {
            reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        cancellationTokenSource?.Cancel();

        cancellationTokenSource = cancellationTokenSource == null
        || cancellationTokenSource.Token.IsCancellationRequested ?
        new CancellationTokenSource() : cancellationTokenSource;

        RenderProbeAsync(cancellationTokenSource.Token).Forget();
    }

    private void OnTriggerExit(Collider other)
    {
        cancellationTokenSource?.Cancel();
    }

    private async UniTaskVoid RenderProbeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (true)
        {
            reflectionProbe?.RenderProbe();

            await UniTask.WaitUntil(() => reflectionProbe.IsFinishedRendering(reflectionProbeId),
            cancellationToken: cancellationToken);

            //await UniTask.NextFrame(cancellationToken);
        }
    }
}
