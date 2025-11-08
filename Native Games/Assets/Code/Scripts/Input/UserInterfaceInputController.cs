using UnityEngine;

public class UserInterfaceInputController
{
    private GameInputs gameInputs;

    public UserInterfaceInputController(GameInputs gameInputs)
    {
        this.gameInputs = gameInputs;
    }

    public bool CancelPressed
    {
        get
        {
            return gameInputs.UI.Cancel.WasPressedThisFrame();
        }
    }

    public bool InventoryPressed
    {
        get
        {
            return gameInputs.UI.Inventory.WasPressedThisFrame();
        }
    }

    public Vector2 PointerPosition
    {
        get
        {
            return gameInputs.UI.Point.ReadValue<Vector2>();
        }
    }
}