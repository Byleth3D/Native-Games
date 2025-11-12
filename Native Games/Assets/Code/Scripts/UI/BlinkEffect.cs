using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Image target;
    [SerializeField] private float toTransparentDuration;
    [SerializeField] private float toOpaqueDuration;
    [SerializeField] private float toTransparentDelay;
    [SerializeField] private float toOpaqueDelay;
    private Sequence alphaTweenSequence;

    private void OnEnable()
    {
        alphaTweenSequence = Sequence.Create(cycles: -1, cycleMode: CycleMode.Yoyo, updateType: UpdateType.Update)
            .Chain(Tween.Delay(toTransparentDelay))
            .Chain(Tween.Alpha(target, 0.0f, duration: toTransparentDuration))
            .Chain(Tween.Delay(toOpaqueDelay))
            .Chain(Tween.Alpha(target, 1.0f, duration: toOpaqueDuration));
    }

    private void OnDisable()
    {
        alphaTweenSequence.Stop();
    }
}
