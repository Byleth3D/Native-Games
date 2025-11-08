
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
}
