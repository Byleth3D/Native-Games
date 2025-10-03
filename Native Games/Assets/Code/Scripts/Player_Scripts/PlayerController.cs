using EditorAttributes;
using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundChecker))]

public class PlayerController : MonoBehaviour
{
    [Header("References", order = 0)]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody body;
    [SerializeField] private CapsuleCollider collider3D;
    [SerializeField] private GameObject gameplayCamera;
    [SerializeField] private GroundChecker groundChecker;

    [Header("Motion")]
    [ShowInInspector] private Vector3 velocity;
    public Vector3 Velocity => velocity;

    [Header("Horizontal Movement")]
    [SerializeField] private float horizontalSpeed = 3.5f;
    [SerializeField] private float pushingSpeed = 3.5f;

    private Vector3 moveDirectionRaw;
    private Vector3 relativeMoveDirection;

    private bool canMove = true;

    [Header("Jump")]
    [SerializeField] private float maxJumpHeight = 2.5f;
    [SerializeField] private float jumpPeakTime = 0.45f;
    [SerializeField] private float jumpFallTime = 0.35f;
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
    public float MaxInteractionDistance => collider3D.radius;

    private InteractionTrigger interactionTrigger;
    private Vector3 interactionCenter;

    private GameObject interactableGameObject;
    private Vector3 interactableDirection;

    public bool IsInteracting { get; protected set; }

    #region Unity Methods
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

    private void Update()
    {
        coyoteTimer.Tick(Time.deltaTime);
        jumpBufferTimer.Tick(Time.deltaTime);
        Jump();
        Interaction();
        Rotate();
    }

    private void FixedUpdate()
    {
        ProcessMove();
        PushObject();
        ProcessGravity();
        ProcessJump();
        ApplyVelocity();
    }
    #endregion

    #region Core Loop Methods
    private void SetJumpSettings()
    {
        jumpGravity = -2.0f * maxJumpHeight / (jumpPeakTime * jumpPeakTime);
        fallGravity = -2.0f * maxJumpHeight / (jumpFallTime * jumpFallTime);
        initialJumpVelocity = 2.0f * maxJumpHeight / jumpPeakTime;
        coyoteTimer = new CountdownTimer(coyoteDuration);
        jumpBufferTimer = new CountdownTimer(jumpBufferDuration);
    }

    private void Rotate()
    {
        Quaternion currentModelRotation = model.localRotation;
        Quaternion targetRotation;
        Vector3 targetDirection;
        float rotationStep = angularSpeed * Time.deltaTime;

        if (IsInteracting)
        {
            interactableDirection = interactableGameObject.transform.position - transform.position;
            interactableDirection.y = 0.0f;
            interactableDirection.Normalize();

            targetDirection = interactableDirection;
        }
        else
        {
            if (relativeMoveDirection.magnitude <= 0.0f) return;

            targetDirection = relativeMoveDirection;
        }

        targetRotation = Quaternion.LookRotation(targetDirection);
        model.localRotation = Quaternion.RotateTowards(currentModelRotation, targetRotation, rotationStep);
    }

    private void Jump()
    {
        if (InputManager.Instance.JumpPressed)
        {
            if (groundChecker.IsGrounded)
            {
                canJump = true;
                animator.SetTrigger("Jump");
            }
            else
            {
                if (coyoteTimer.IsRunning && body.linearVelocity.y < 0.0f)
                {
                    canJump = true;
                    coyoteTimer.Stop();
                    animator.SetTrigger("Jump");
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
                animator.SetTrigger("Jump");
                jumpBufferTimer.Stop();
            }
        }
    }
    #endregion

    #region Physics Loop Methods
    private void ProcessMove()
    {
        if (canMove)
        {
            MoveHorizontally();
        }
        else
        {
            if (!IsInteracting)
            {
                return;
            }

            velocity = Vector3.zero;
            Teleport();
        }

        animator.SetBool("Run", velocity.WithoutY().magnitude > 0.0f);
    }

    private void MoveHorizontally()
    {
        Vector2 motionInput = InputManager.Instance.MotionInput;
        Vector3 previousMoveDirection = moveDirectionRaw;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);
        float currentHorizontalSpeed = horizontalSpeed;

        if (IsInteracting && interactionTrigger.GetInteractionType() == InteractionType.Push)
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
            currentHorizontalSpeed = pushingSpeed;
        }

        Vector3 cameraForwardDirection = gameplayCamera.transform.forward;
        cameraForwardDirection.y = 0.0f;
        cameraForwardDirection.Normalize();

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForwardDirection);

        relativeMoveDirection = cameraForwardRotation * moveDirectionRaw;

        velocity.x = relativeMoveDirection.x * currentHorizontalSpeed;
        velocity.z = relativeMoveDirection.z * currentHorizontalSpeed;
    }

    private void Teleport()
    {
        Vector3 position = interactionCenter;

        Teleport(position);
        canMove = true;
    }

    private void Teleport(Vector3 position)
    {
        position.y = body.position.y;
        body.position = position;
    }

    private void PushObject()
    {
        if (interactionTrigger == null || !canMove)
        {
            return;
        }

        if (!IsInteracting || interactionTrigger.GetInteractionType() != InteractionType.Push)
        {
            animator.SetBool("Push", false);
            animator.SetBool("Pull", false);
            return;
        }

        float dot = Vector3.Dot(velocity.WithoutY().normalized, model.transform.forward);
        Debug.Log($"{dot}");

        if (dot < 0.0f)
        {
            animator.SetBool("Pull", true);
        }
        else if (dot >= 0.0f)
        {
            animator.SetBool("Pull", false);
        }

        interactionTrigger.TriggerInteract(true);
    }

    private void ProcessGravity()
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

    private void ProcessJump()
    {
        if (!canJump)
        {
            return;
        }

        body.AddForce(Vector3.down * body.linearVelocity.y, ForceMode.VelocityChange);

        velocity.y = initialJumpVelocity;
        canJump = false;
    }

    private void ApplyVelocity()
    {
        body.AddForce(velocity - body.linearVelocity, ForceMode.VelocityChange);
    }
    #endregion

    #region Ground Response Methods
    private void OnGroundEnter()
    {
        coyoteTimer.Stop();
        animator.SetBool("IsGrounded", true);
    }

    private void OnGroundExit()
    {
        coyoteTimer.Start();
        animator.SetBool("IsGrounded", false);
    }
    #endregion

    #region Interaction Methods
    public void InteractionEnter(InteractionTrigger interactionTrigger)
    {
        this.interactionTrigger = interactionTrigger;
        interactableGameObject = interactionTrigger.transform.parent.gameObject;
        interactionCenter = interactionTrigger.ActiveTrigger.bounds.center;
    }

    public void Interaction()
    {
        if (groundChecker.IsGrounded && interactionTrigger)
        {
            if (interactionTrigger.GetInteractionType() == InteractionType.Push)
            {
                if (InputManager.Instance.InteractHeld && !IsInteracting)
                {
                    IsInteracting = true;
                    canMove = false;
                    animator.SetBool("Push", true);
                }
                else if (!InputManager.Instance.InteractHeld && IsInteracting)
                {
                    InteractionCancel();
                    canMove = true;
                    animator.SetBool("Push", false);
                    animator.SetBool("Pull", false);
                }
            }
            else
            {
                if (InputManager.Instance.InteractPressed)
                {
                    if (interactionTrigger.GetInteractionType() == InteractionType.Collect)
                    {
                        animator.SetTrigger("Pick");
                        InputManager.Instance.DisableAction("Move", 2.3f);
                    }

                    interactionTrigger.TriggerInteract(false);
                }
            }
        }
        else if (!groundChecker.IsGrounded && IsInteracting)
        {
            InteractionCancel();
            interactionTrigger.TriggerInteractCancel(true);
            InputManager.Instance.DisableAction("Interact", 0.5f);
        }
    }

    public void InteractionCancel()
    {
        IsInteracting = false;
        interactionTrigger.TriggerInteractCancel(true);
    }

    public void InteractionExit()
    {
        interactionTrigger = null;
        interactableGameObject = null;
        interactionCenter = Vector3.zero;
        IsInteracting = false;
        animator.SetBool("Push", false);
        animator.SetBool("Pull", false);
    }
    #endregion
}