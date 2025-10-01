using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


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

    private List<CountdownTimer> disabledActionsTimers = new();

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
        foreach (CountdownTimer timer in disabledActionsTimers)
        {
            timer.Tick(Time.deltaTime);
        }

        MotionInput = moveAction.ReadValue<Vector2>();
        JumpPressed = jumpAction.WasPressedThisFrame();

        if (interactAction.enabled)
        {
            InteractPressed = interactAction.WasPressedThisFrame();

            if (!InteractPressed && interactAction.enabled)
            {
                InteractHeld = interactAction.phase == InputActionPhase.Performed ? true : false;
            }
        }
        else
        {
            InteractPressed = false;
            InteractHeld = false;
        }

    }

    public void EnableAction(string actionToEnable)
    {
        InputAction action = gameInputs.FindAction(actionToEnable);

        if (action == null) return;

        EnableAction(action);
    }

    private void EnableAction(InputAction actionToEnable)
    {
        actionToEnable.Enable();
    }

    public void DisableAction(string actionToDisable)
    {
        InputAction action = gameInputs.FindAction(actionToDisable);

        if (action == null) return;

        DisableAction(action);
    }

    public void DisableAction(string actionToDisable, float duration)
    {
        InputAction action = gameInputs.FindAction(actionToDisable);

        if (action == null) return;

        DisableAction(action);

        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnableAction(action);
        timer.Start();

        disabledActionsTimers.Add(timer);
    }

    private void DisableAction(InputAction actionToDisable)
    {
        actionToDisable.Disable();
    }
}
