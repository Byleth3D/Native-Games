using EditorAttributes;
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

    [Header("General")]
    [SerializeField] private float inputDisableDuration = 0.25f;

    [Header("Motion")]
    [ShowInInspector] private Vector3 velocity;
    public Vector3 Velocity => velocity;

    [Header("Horizontal Movement")]
    [SerializeField] private float horizontalSpeed = 3.5f;
    [SerializeField] private float pushingSpeed = 3.5f;
    [SerializeField, Range(0.01f, 1f)] private float airMultiplier = 0.75f;

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
    public InteractionTrigger InteractionTrigger { get; set; }
    private Vector3 interactionCenter;

    private GameObject interactableGameObject;
    private Vector3 interactableDirection;

    public bool IsPushing { get; protected set; }

    public bool IsAlive { get; private set; } = true;

    public Vector3 Forward => model.transform.forward;

    #region Unity Methods
    private void Awake()
    {
        body.maxLinearVelocity = 80f;
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

        if (IsAlive)
        {
            Jump();
            Interaction();
            Rotate();
            animator.SetBool("IsGrounded", groundChecker.IsGrounded);
        }
    }

    private void FixedUpdate()
    {
        if (IsAlive)
        {
            ProcessMove();
            PushObject();
            ProcessGravity();
            ProcessJump();
            ApplyVelocity();
        }
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
        Vector3 targetDirection = Vector3.zero;
        float rotationStep = angularSpeed * Time.deltaTime;


        if (InteractionTrigger && (InputManager.Instance.Player.InteractPressed || IsPushing))
        {
            interactableDirection = interactableGameObject.transform.position - transform.position;
            interactableDirection.y = 0.0f;
            interactableDirection.Normalize();

            targetDirection = interactableDirection;
        }
        else
        {
            if (relativeMoveDirection.magnitude <= 0.0f)
            {
                return;
            }

            targetDirection = relativeMoveDirection;
        }

        targetRotation = Quaternion.LookRotation(targetDirection);

        if (InteractionTrigger == null || (IsPushing && InteractionTrigger.GetInteractionType() == InteractionType.Push))
        {
            model.localRotation = Quaternion.RotateTowards(currentModelRotation, targetRotation, rotationStep);
        }
        else
        {
            model.localRotation = targetRotation;
        }
    }

    private void Jump()
    {
        if (InputManager.Instance.Player.JumpPressed)
        {
            if (groundChecker.IsGrounded)
            {
                canJump = true;
                animator.SetTrigger("Jump");
                animator.SetBool("IsGrounded", false);
            }
            else
            {
                if (coyoteTimer.IsRunning && body.linearVelocity.y < 0.0f)
                {
                    canJump = true;
                    coyoteTimer.Stop();
                    animator.SetTrigger("Jump");
                    animator.SetBool("IsGrounded", false);
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
                animator.SetBool("IsGrounded", false);
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
            if (!IsPushing)
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
        Vector2 motionInput = InputManager.Instance.Player.MotionInput;
        Vector3 previousMoveDirection = moveDirectionRaw;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);

        float currentHorizontalSpeed = horizontalSpeed;

        if (!groundChecker.IsGrounded)
        {
            currentHorizontalSpeed *= airMultiplier;
        }
        else if (IsPushing && InteractionTrigger.GetInteractionType() == InteractionType.Push)
        {
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
        position.y = body.position.y;

        body.position = position;
        canMove = true;
    }

    public void Teleport(Vector3 position)
    {
        body.position = position;
        canMove = true;
    }

    private void PushObject()
    {
        if (InteractionTrigger == null || !canMove)
        {
            return;
        }

        if (!IsPushing || InteractionTrigger.GetInteractionType() != InteractionType.Push)
        {
            animator.SetBool("Push", false);
            animator.SetBool("Pull", false);
            return;
        }

        float dot = Vector3.Dot(velocity.WithoutY().normalized, model.transform.forward);

        if (dot < 0.0f)
        {
            animator.SetBool("Pull", true);
        }
        else if (dot >= 0.0f)
        {
            animator.SetBool("Pull", false);
        }

        InteractionTrigger.TriggerInteract();
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

    #region Death Response
    public void SetAsAlive()
    {
        if (!IsAlive)
        {
            IsAlive = true;
            animator.SetTrigger("Alive");
            animator.SetBool("IsGrounded", true);
        }

        InputManager.Instance.DisableInputActions(InputControllerType.Player, inputDisableDuration);
    }

    public void SetAsDead()
    {
        IsAlive = false;

        coyoteTimer.Stop();
        jumpBufferTimer.Stop();

        animator.SetTrigger("FallingDeath");
        animator.SetBool("Run", false);
        animator.SetBool("Pull", false);
        animator.SetBool("Push", false);
        animator.SetBool("IsGrounded", false);
    }
    #endregion

    #region Ground Response Methods
    private void OnGroundEnter()
    {
        coyoteTimer.Stop();

        if (IsAlive)
        {
            animator.SetBool("IsGrounded", true);
        }
    }

    private void OnGroundExit()
    {
        coyoteTimer.Start();

        if (IsAlive)
        {
            animator.SetBool("IsGrounded", false);
        }
    }
    #endregion

    #region Interaction Methods
    public void InteractionEnter(InteractionTrigger interactionTrigger)
    {
        this.InteractionTrigger = interactionTrigger;
        interactableGameObject = interactionTrigger.transform.parent.gameObject;
        interactionCenter = interactionTrigger.ActiveTrigger.bounds.center;
    }

    public void Interaction()
    {
        if (groundChecker.IsGrounded && InteractionTrigger)
        {
            if (InteractionTrigger.GetInteractionType() == InteractionType.Push)
            {
                if (InputManager.Instance.Player.InteractPressed)
                {
                    if (!IsPushing)
                    {
                        IsPushing = true;
                        canMove = false;
                        animator.SetBool("Push", true);
                    }
                    else
                    {
                        InteractionCancel();
                        canMove = true;
                    }
                }
            }
            else
            {
                if (InputManager.Instance.Player.InteractPressed)
                {
                    InteractionTrigger.TriggerInteract();

                    if (InteractionTrigger.GetInteractionType() == InteractionType.Collect && InteractionTrigger.HasInteraction)
                    {
                        animator.SetTrigger("Pick");
                        InputManager.Instance.DisableInputActions(InputControllerType.Player, 2.3f);
                    }
                }
            }
        }
        else if (!groundChecker.IsGrounded && IsPushing)
        {
            InteractionCancel();
            InteractionTrigger.TriggerInteractCancel(true);
            InputManager.Instance.DisableAction("Push", 0.5f);
        }
    }

    public void InteractionCancel()
    {
        IsPushing = false;
        InteractionTrigger.TriggerInteractCancel(true);
        animator.SetBool("Push", false);
        animator.SetBool("Pull", false);
    }

    public void InteractionExit()
    {
        InteractionTrigger = null;
        interactableGameObject = null;
        interactionCenter = Vector3.zero;
        IsPushing = false;
        animator.SetBool("Push", false);
        animator.SetBool("Pull", false);
    }
    #endregion
}