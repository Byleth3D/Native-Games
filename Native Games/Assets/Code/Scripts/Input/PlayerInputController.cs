using UnityEngine;

public class PlayerInputController
{
    private GameInputs gameInputs;

    public PlayerInputController(GameInputs gameInputs)
    {
        this.gameInputs = gameInputs;
    }

    public Vector2 MotionInput
    {
        get
        {
            return gameInputs.Player.Move.ReadValue<Vector2>();
        }
    }

    public bool JumpPressed
    {
        get
        {
            return gameInputs.Player.Jump.WasPressedThisFrame();
        }
    }

    public bool InteractPressed
    {
        get
        {
            return gameInputs.Player.Interact.WasPressedThisFrame();
        }
    }
}