
public class GameplayUIManager : LocalSingleton<GameplayUIManager>
{
    public InGameMenuManager inGameMenuManager;
    public PopupManager popupManager;
    public InventoryMenuManager inventoryMenuManager;

    protected override void Awake()
    {
        base.Awake();
        inGameMenuManager.Setup();
        popupManager.Setup();
    }

    private void Update()
    {
        if (InputManager.Instance.CancelPressed)
        {
            if (inGameMenuManager.HasOverlappingMenu || inGameMenuManager.HasActiveMenu)
            {
                inGameMenuManager.DisableActiveMenu();
                return;
            }
            else
            {
                inGameMenuManager.EnableInGameMenu("Pause");
                return;
            }
        }

        if (InputManager.Instance.InventoryPressed)
        {
            if (inGameMenuManager.HasOverlappingMenu)
            {
                return;
            }

            if (!inGameMenuManager.HasActiveMenu)
            {
                inGameMenuManager.EnableInGameMenu("Inventory");
            }
            else if (inGameMenuManager.HasActiveMenu && inGameMenuManager.ActiveMenu.inGameMenuName == "Inventory")
            {
                inGameMenuManager.DisableActiveMenu();
            }
        }
    }

    public void EnableInGameMenu(string inGameMenu)
    {
        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        inGameMenuManager.DisableInGameMenu(inGameMenu);
    }
}