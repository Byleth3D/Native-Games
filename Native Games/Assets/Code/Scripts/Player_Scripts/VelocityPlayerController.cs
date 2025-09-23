using EditorAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class VelocityPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private Rigidbody rigidBody;

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
    [SerializeField] private float coyoteDuration;
    [SerializeField] private float jumpBufferDuration;

    private float initialJumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private CountdownTimer coyoteTimer;
    private CountdownTimer jumpBufferTimer;

    private bool canJump;

    [Header("Rotate")]
    [SerializeField] private float angularSpeed = 360.0f;
    [SerializeField] private Transform model;
    
    [Header("Interaction")]
    private bool isPushingObject;

    private void Awake()
    {
        SetJumpSettings();
        coyoteTimer = new CountdownTimer(coyoteDuration);
        jumpBufferTimer = new CountdownTimer(jumpBufferDuration);
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
        coyoteTimer.Tick(Time.deltaTime);
        jumpBufferTimer.Tick(Time.deltaTime);
        Jump();
        Rotate();
        Interact();
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
        Vector3 previousMoveDirection = moveDirectionRaw;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);

        if (isPushingObject)
        {
            float absX = Mathf.Abs(moveDirectionRaw.x);
            float previousAbsX = Mathf.Abs(previousMoveDirection.x);

            float absZ = Mathf.Abs(moveDirectionRaw.z);
            float previousAbsZ = Mathf.Abs(previousMoveDirection.z);

            if (absX > 0.0f && absZ > 0.0f)
            {
                if (previousAbsX > 0.0f)
                {
                    moveDirectionRaw.z = 0.0f;
                }
                else if (previousAbsZ > 0.0f)
                {
                    moveDirectionRaw.x = 0.0f;
                }
            }

            moveDirectionRaw.Normalize();
        }

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
        if (relativeMoveDirection.magnitude <= 0.0f || isPushingObject) return;

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
                if (coyoteTimer.IsRunning && rigidBody.linearVelocity.y < 0.0f)
                {
                    canJump = true;
                    coyoteTimer.Stop();
                    Debug.Log("Coyote");
                    return;
                }

                jumpBufferTimer.Start();
            }
        }
        else
        {
            if (groundChecker.IsGrounded && jumpBufferTimer.IsRunning)
            {
                canJump = true;
                jumpBufferTimer.Stop();
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

    private void Interact()
    {
        if (groundChecker.IsGrounded)
        {
            if (InputManager.Instance.InteractPressed && !InputManager.Instance.InteractHeld)
            {
                return;
            }

            if (InputManager.Instance.InteractHeld)
            {
                isPushingObject = !isPushingObject;
            }
        }
    }

    private void OnGroundEnter()
    {
        coyoteTimer.Stop();
    }

    private void OnGroundExit()
    {
        coyoteTimer.Start();
    }
}
