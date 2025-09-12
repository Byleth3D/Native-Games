using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private GameInputs gameInputs;

    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction moveAction;

    public Vector2 MotionInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool InteractHeld { get; private set; }

    private void OnEnable()
    {
        if (this != Instance) return;

        gameInputs ??= new GameInputs();
        gameInputs.Player.Enable();
        interactAction = gameInputs.Player.Interact;
        jumpAction = gameInputs.Player.Jump;
        moveAction = gameInputs.Player.Move;
    }

    private void OnDisable()
    {
        if (this != Instance) return;

        gameInputs.Player.Disable();
    }

    private void Update()
    {
        MotionInput = moveAction.ReadValue<Vector2>();
        JumpPressed = jumpAction.WasPressedThisFrame();
        InteractPressed = interactAction.WasPressedThisFrame();
        InteractHeld = interactAction.WasCompletedThisFrame();
    }
}
