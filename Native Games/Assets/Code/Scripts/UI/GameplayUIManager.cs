using UnityEngine;

public class GameplayUIManager : LocalSingleton<GameplayUIManager>
{
    [SerializeField] private GameObject mobileHUD;
    public InGameMenuManager inGameMenuManager;
    public PopupManager popupManager;
    public InventoryMenuManager inventoryMenuManager;

    private void Start()
    {
        inGameMenuManager.Setup();
        popupManager.Setup();

#if !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.Android
    || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            mobileHUD.SetActive(true);
        }
        else
        {
            mobileHUD.SetActive(false);
        }
#endif

        inventoryMenuManager.Setup();
    }

    private void Update()
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        if (InputManager.Instance.UI.CancelPressed)
        {
            if (inGameMenuManager.HasOverlappingMenu || inGameMenuManager.HasActiveMenu)
            {
                inGameMenuManager.DisableActiveMenu();
                return;
            }
            else
            {
                EnableInGameMenu("Pause");
                return;
            }
        }

        if (InputManager.Instance.UI.InventoryPressed)
        {
            if (inGameMenuManager.HasOverlappingMenu)
            {
                return;
            }

            if (!inGameMenuManager.HasActiveMenu)
            {
                EnableInGameMenu("Inventory");
            }
            else if (inGameMenuManager.HasActiveMenu && inGameMenuManager.ActiveMenu.inGameMenuName == "Inventory")
            {
                inGameMenuManager.DisableActiveMenu();
            }
        }
    }

    public void EnableInGameMenu(string inGameMenu)
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        if (inGameMenu == "Inventory")
        {
            inventoryMenuManager.Notification.SetActive(false);
        }

        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        if (ScreenFader.Instance.IsFading || LoadingScreen.Instance.IsVisible)
        {
            return;
        }

        inGameMenuManager.DisableInGameMenu(inGameMenu);
    }

    public void DisableActiveInGameMenu()
    {
        inGameMenuManager.DisableActiveMenu();
    }

    public void RestartCheckpoint()
    {
        GameManager.Instance.SwitchState(GameState.Running);
        InputManager.Instance.DisableInputActions(InputControllerType.Player);
        SceneLoader.Instance.Load(LoadingType.RestartCheckpoint);
    }

    public void ExitGame()
    {
        GameManager.Instance.SwitchState(GameState.Exiting);
    }
}