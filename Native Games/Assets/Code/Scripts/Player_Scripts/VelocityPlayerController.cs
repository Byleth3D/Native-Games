using EditorAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class VelocityPlayerController : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField] GroundChecker groundChecker;

    [SerializeField]
    private Rigidbody rigidBody;

    [Header("Motion")]
    [ShowInInspector] private Vector3 velocity;

    [Header("Horizontal Movement")]
    [SerializeField, Range(0.0f, 100.0f)] float horizontalSpeed = 3.5f;
    private Vector3 moveDirectionRaw;
    private Vector3 relativeMoveDirection;

    [Header("Jump")]
    [SerializeField, Range(0.1f, 100f)] private float maxJumpHeight = 2.5f;
    [SerializeField, Range(0.01f, 60f)] private float jumpPeakTime = 0.45f;
    [SerializeField, Range(0.01f, 60f)] private float jumpFallTime = 0.35f;
    [SerializeField] private CountdownTimer jumpBuffer;
    [SerializeField] private CountdownTimer coyoteTime;
    private float initialJumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private bool canJump;

    [Header("Rotate")]
    [SerializeField] private float angularSpeed = 360.0f;
    [SerializeField] private Transform model;

    private void Awake()
    {
        SetJumpSettings();
    }

    private void OnValidate()
    {
        SetJumpSettings();
    }

    private void OnEnable()
    {
        groundChecker.OnGroundEnter += OnGroundEnter;
        groundChecker.OnGroundExit += OnGroundExit;
    }

    private void OnDisable()
    {
        groundChecker.OnGroundEnter -= OnGroundEnter;
        groundChecker.OnGroundExit -= OnGroundExit;
    }

    private void SetJumpSettings()
    {
        jumpGravity = -2.0f * maxJumpHeight / (jumpPeakTime * jumpPeakTime);
        fallGravity = -2.0f * maxJumpHeight / (jumpFallTime * jumpFallTime);
        initialJumpVelocity = 2.0f * maxJumpHeight / jumpPeakTime;
    }

    private void Update()
    {
        coyoteTime.Tick(Time.deltaTime);
        jumpBuffer.Tick(Time.deltaTime);
        Jump();
        Rotate();
    }

    private void FixedUpdate()
    {
        MoveHorizontally();
        ApplyGravity();
        ProcessJump();
        ApplyVelocity();
    }

    private void MoveHorizontally()
    {
        Vector2 motionInput = InputManager.Instance.MotionInput;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);

        Vector3 cameraForwardDirection = cinemachineCamera.transform.forward;
        cameraForwardDirection.y = 0.0f;
        cameraForwardDirection.Normalize();

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForwardDirection);

        relativeMoveDirection = cameraForwardRotation * moveDirectionRaw;

        velocity.x = relativeMoveDirection.x * horizontalSpeed;
        velocity.z = relativeMoveDirection.z * horizontalSpeed;
    }

    private void Rotate()
    {
        if (relativeMoveDirection.magnitude <= 0.0f) return;

        Quaternion currentModelRotation = model.localRotation;
        Quaternion targetRotation = Quaternion.LookRotation(relativeMoveDirection);
        float rotationStep = angularSpeed * Time.deltaTime;
        model.localRotation = Quaternion.RotateTowards(currentModelRotation, targetRotation, rotationStep);
    }

    private void ApplyGravity()
    {
        if (groundChecker.IsGrounded)
        {
            velocity.y = 0.0f;
        }
        else
        {
            float currentGravity = velocity.y > 0.0f ? jumpGravity : fallGravity;
            velocity.y += currentGravity * Time.deltaTime;
        }
    }

    private void ApplyVelocity()
    {
        rigidBody.AddForce(velocity - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        if (InputManager.Instance.JumpPressed)
       {
            if (groundChecker.IsGrounded)
            {
                canJump = true;
             }
            else
            {
                if (coyoteTime.IsRunning && rigidBody.linearVelocity.y < 0.0f)
                {
                    canJump = true;
                    coyoteTime.Stop();
                    Debug.Log("Coyote");
                    return;
                }

                jumpBuffer.Start();
            }
        }
        else
        {
            if (groundChecker.IsGrounded && jumpBuffer.IsRunning)
            {
                canJump = true;
                jumpBuffer.Stop();
                Debug.Log("Buffered");
            }
        }
    }

    private void ProcessJump()
    {
        if (!canJump) return;
        rigidBody.AddForce(Vector3.down * rigidBody.linearVelocity.y, ForceMode.VelocityChange);
        velocity.y = initialJumpVelocity;
        canJump = false;
    }

    private void OnGroundEnter()
    {
        coyoteTime.Stop();
    }

    private void OnGroundExit()
    {
        coyoteTime.Start();
    }
}
