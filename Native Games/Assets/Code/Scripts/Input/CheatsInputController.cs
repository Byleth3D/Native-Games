
using UnityEngine;

public class CheatsInputController
{
    private GameInputs gameInputs;

    public CheatsInputController(GameInputs gameInputs)
    {
        this.gameInputs = gameInputs;
    }

    public bool ModifierPressed
    {
        get
        {
            return gameInputs.Cheats.CheatModifier.IsPressed();
        }
    }

    public bool TeleportNextPressed
    {
        get
        {
            return gameInputs.Cheats.TeleportNext.WasPressedThisFrame();
        }
    }

    public bool TeleportPreviousPressed
    {
        get
        {
            return gameInputs.Cheats.TeleportPrevious.WasPressedThisFrame();
        }
    }

    public bool FreeMode
    {
        get
        {
            return gameInputs.Cheats.FreeMode.WasPressedThisFrame();
        }
    }

    public Vector2 FreeMove
    {
        get
        {
            return gameInputs.Cheats.FreeMove.ReadValue<Vector2>();
        }
    }

    public Vector2 FreePoint
    {
        get
        {
            return gameInputs.Cheats.FreePoint.ReadValue<Vector2>();
        }
    }
}
