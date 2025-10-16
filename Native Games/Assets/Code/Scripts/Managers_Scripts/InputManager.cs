using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class InputManager : Singleton<InputManager>
{
    private GameInputs gameInputs;

    #region Gameplay Fields
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction pushAction;

    public Vector2 MotionInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool PushHeld { get; private set; }
    #endregion

    private InputAction submitAction;
    private InputAction cancelAction;
    private InputAction pointAction;
    private InputAction inventoryAction;

    public bool CancelPressed { get; private set; }
    public bool InventoryPressed { get; private set; }

    private List<CountdownTimer> disabledActionsTimers = new();

    private void OnEnable()
    {
        if (this != Instance) return;

        gameInputs ??= new GameInputs();
        gameInputs.Player.Enable();
        gameInputs.UI.Enable();
        interactAction = gameInputs.Player.Interact;
        jumpAction = gameInputs.Player.Jump;
        moveAction = gameInputs.Player.Move;
        pushAction = gameInputs.Player.Push;
        cancelAction = gameInputs.UI.Cancel;
        inventoryAction = gameInputs.UI.Inventory;
        pointAction = gameInputs.UI.Point;
    }

    private void OnDisable()
    {
        if (this != Instance) return;

        gameInputs.Player.Disable();
        gameInputs.UI.Disable();
    }

    private void Update()
    {
        foreach (CountdownTimer timer in disabledActionsTimers)
        {
            timer.Tick(Time.deltaTime);
        }

        GetPlayerInputs();
        GetUserInterfaceInputs();
    }

    private void GetPlayerInputs()
    {
        MotionInput = moveAction.ReadValue<Vector2>();
        JumpPressed = jumpAction.WasPressedThisFrame();
        InteractPressed = interactAction.WasPressedThisFrame();

        if (pushAction.enabled)
        {
            PushHeld = pushAction.phase == InputActionPhase.Performed ? true : false;
        }
        else
        {
            PushHeld = false;
        }
    }

    private void GetUserInterfaceInputs()
    {
        CancelPressed = cancelAction.WasPressedThisFrame();
        InventoryPressed = inventoryAction.WasPressedThisFrame();
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

    public void EnablePlayerActions()
    {
        gameInputs.Player.Enable();
    }

    public void DisablePlayerActions()
    {
        gameInputs.Player.Disable();
    }

    public void DisablePlayerActions(float duration)
    {
        gameInputs.Player.Disable();
        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnablePlayerActions();
        timer.Start();

        disabledActionsTimers.Add(timer);
    }

    public Vector2 GetPointerPosition()
    {
        return pointAction.ReadValue<Vector2>();
    }
}
