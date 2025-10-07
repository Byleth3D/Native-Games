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
    }

    private void OnEnable()
    {
        groundChecker.OnGroundExit += OnGroundExit;
    }

    private void OnDisable()
    {
        groundChecker.OnGroundExit -= OnGroundExit;
    }

    private void Update()
    {
        if (groundChecker.IsGrounded)
        {
            if (stepTimer.IsRunning)
            {
                stepTimer.Tick(Time.deltaTime);
                return;
            }

            if (controller.Velocity.WithoutY().magnitude > 0.0f)
            {
                PlayStep();
            }
            else
            {
                stepTimer.Stop();
                Debug.Log($"Stopped Zero Vel | Time: {Time.time}");
                return;
            }
        }
        else
        {
            stepTimer.Stop();
        }
    }

    private void PlayStep()
    {
        if (controller.Velocity.WithoutY().magnitude > 0.0f && !controller.IsInteracting)
        {
            PlayClip(steps);
            Debug.Log($"Step | Grounded: {groundChecker.IsGrounded} | Time: {Time.time}" +
                $"Timer Running: {stepTimer.IsRunning}");
            stepTimer.Start();
            Debug.Log($"Timer Running: {stepTimer.IsRunning}");
        }
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

    private void OnGroundExit()
    {
        PlayJump();
    }
}