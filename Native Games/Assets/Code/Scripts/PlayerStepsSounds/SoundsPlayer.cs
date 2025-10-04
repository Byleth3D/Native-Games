using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    private PlayerController controller;
    private GroundChecker groundChecker;
    private AudioSource audioSource;

    public AudioClip steps;
    public AudioClip jump;

    public float stepsInterval = 0.5f;

    private CountdownTimer stepTimer;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        groundChecker = GetComponent<GroundChecker>();
        audioSource = GetComponent<AudioSource>();

        stepTimer = new(stepsInterval);
        stepTimer.Start();
    }

    private void OnEnable()
    {
        groundChecker.OnGroundExit += PlayJump;
        stepTimer.OnTimerExpired += PlayStep;
    }

    private void OnDisable()
    {
        groundChecker.OnGroundExit -= PlayJump;
        stepTimer.OnTimerExpired -= PlayStep;
    }

    private void Update()
    {
        if (groundChecker.IsGrounded && stepTimer.IsRunning)
        {
            stepTimer.Tick(Time.deltaTime);
        }
    }

    private void PlayStep()
    {
        if (controller.Velocity.WithoutY().magnitude > 0f && !controller.IsInteracting)
        {
            PlayClip(steps);
        }

        stepTimer.Start();
    }

    private void PlayJump()
    {
        if (controller.Velocity.y > 0.0f)
        {
            PlayClip(jump);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}