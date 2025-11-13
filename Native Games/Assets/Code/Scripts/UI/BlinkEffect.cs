using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private bool unscaledTime = false;
    [SerializeField] private Image target;

    [SerializeField] private float duration;
    [SerializeField] private float transparencyDelay;
    [SerializeField] private float opacityDelay;

    private Sequence alphaTweenSequence;

    private void OnEnable()
    {
        alphaTweenSequence = Sequence
            .Create(cycles: -1, cycleMode: CycleMode.Restart,
            updateType: UpdateType.Update, useUnscaledTime: unscaledTime)
            .Chain(Tween.Delay(transparencyDelay))
            .Chain(Tween.Alpha(target, 0.0f, duration: duration))
            .Chain(Tween.Delay(opacityDelay))
            .Chain(Tween.Alpha(target, 1f, duration: duration));
    }

    private void OnDisable()
    {
        alphaTweenSequence.Stop();
    }
}
