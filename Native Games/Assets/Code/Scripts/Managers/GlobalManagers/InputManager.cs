using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private GameInputs gameInputs;
    public bool Enabled { get; private set; }
    private List<CountdownTimer> timers = new();

    public PlayerInputController Player { get; private set; }
    public UserInterfaceInputController UI { get; private set; }
    public CheatsInputController Cheats { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        gameInputs = new GameInputs();
        EnableGameInputs();

        Player = new PlayerInputController(gameInputs);
        UI = new UserInterfaceInputController(gameInputs);
        Cheats = new CheatsInputController(gameInputs);
    }

    //private void Update()
    //{
    //    foreach (CountdownTimer timer in timers)
    //    {
    //        timer.Tick(Time.deltaTime);
    //        //Talvez vou fazer TimerManager
    //    }
    //}

    public void DisableAction(string actionToDisable, float duration)
    {
        if (!Enabled)
        {
            return;
        }

        DisableAction(actionToDisable);

        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnableAction(actionToDisable);
        timer.Start();

        timers.Add(timer);
        Clock.QueueToAdd(timer);
    }

    public void DisableAction(string actionToDisable)
    {
        InputAction action = gameInputs.FindAction(actionToDisable);

        if (action == null) return;
    }

    public void DisableGameInputs(float duration)
    {
        DisableGameInputs();
        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnableGameInputs();
        timer.Start();

        timers.Add(timer);
        Clock.QueueToAdd(timer);
    }

    public void DisableGameInputs()
    {
        gameInputs.Player.Disable();
        gameInputs.UI.Disable();
        gameInputs.Cheats.Disable();
        Enabled = false;
    }

    public void DisableInputActions(InputControllerType inputControllerType, float duration)
    {
        if (!Enabled)
        {
            return;
        }

        DisableInputActions(inputControllerType);
        CountdownTimer timer = new CountdownTimer(duration);

        timer.OnTimerExpired += () => EnableInputActions(inputControllerType);
        timer.Start();

        timers.Add(timer);
        Clock.QueueToAdd(timer);
    }

    public void DisableInputActions(InputControllerType inputControllerType)
    {
        if (!Enabled)
        {
            return;
        }

        if (inputControllerType is InputControllerType.UI)
        {
            gameInputs.UI.Disable();
            return;
        }

        gameInputs.Player.Disable();
    }

    public void EnableAction(string actionToEnable)
    {
        if (!Enabled)
        {
            return;
        }

        InputAction action = gameInputs.FindAction(actionToEnable);

        if (action == null)
        {
            return;
        }

        action.Enable();
    }

    public void EnableGameInputs()
    {
        gameInputs.Player.Enable();
        gameInputs.UI.Enable();
        gameInputs.Cheats.Enable();
        Enabled = true;
    }

    public void EnableInputActions(InputControllerType inputControllerType)
    {
        if (!Enabled)
        {
            return;
        }

        if (inputControllerType is InputControllerType.UI)
        {
            gameInputs.UI.Enable();
            return;
        }

        gameInputs.Player.Enable();
    }
}

public enum InputControllerType
{
    Player, UI
}