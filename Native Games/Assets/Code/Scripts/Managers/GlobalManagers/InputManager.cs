using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class InputManager : Singleton<InputManager>
{
    private GameInputs gameInputs;
    private bool inputEnabled;

    #region Gameplay Fields
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction moveAction;

    public Vector2 MotionInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
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
    }

    private void GetUserInterfaceInputs()
    {
        CancelPressed = cancelAction.WasPressedThisFrame();
        InventoryPressed = inventoryAction.WasPressedThisFrame();
    }

    public void EnableAction(string actionToEnable)
    {
        if (!inputEnabled)
        {
            return;
        }

        InputAction action = gameInputs.FindAction(actionToEnable);

        if (action == null) return;

        EnableAction(action);
    }

    private void EnableAction(InputAction actionToEnable)
    {
        if (!inputEnabled)
        {
            return;
        }

        actionToEnable.Enable();
    }

    public void DisableAction(string actionToDisable)
    {
        if (!inputEnabled)
        {
            return;
        }

        InputAction action = gameInputs.FindAction(actionToDisable);

        if (action == null) return;

        DisableAction(action);
    }

    public void DisableAction(string actionToDisable, float duration)
    {
        if (!inputEnabled)
        {
            return;
        }

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
        if (!inputEnabled)
        {
            return;
        }

        actionToDisable.Disable();
        actionToDisable.Reset();
    }

    public void EnablePlayerActions()
    {
        if (!inputEnabled)
        {
            return;
        }

        gameInputs.Player.Enable();
    }

    public void DisablePlayerActions()
    {
        if (!inputEnabled)
        {
            return;
        }

        gameInputs.Player.Disable();
    }

    public void DisablePlayerActions(float duration)
    {
        if (!inputEnabled)
        {
            return;
        }

        gameInputs.Player.Disable();
        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnablePlayerActions();
        timer.Start();

        disabledActionsTimers.Add(timer);
    }

    public void EnableGameInputs()
    {
        gameInputs.Player.Enable();
        gameInputs.UI.Enable();
        inputEnabled = true;
    }

    public void DisableGameInputs(float duration)
    {
        DisableGameInputs();
        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnableGameInputs();
        timer.Start();

        disabledActionsTimers.Add(timer);
    }

    public void DisableGameInputs()
    {
        gameInputs.Player.Disable();
        gameInputs.UI.Disable();
        inputEnabled = false;
    }

    public Vector2 GetPointerPosition()
    {
        return pointAction.ReadValue<Vector2>();
    }
}
