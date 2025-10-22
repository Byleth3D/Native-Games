using UnityEngine;

public class GameplayUIManager : LocalSingleton<GameplayUIManager>
{
    [SerializeField] private GameObject mobileHUD;
    public InGameMenuManager inGameMenuManager;
    public PopupManager popupManager;
    public InventoryMenuManager inventoryMenuManager;

    protected override void Awake()
    {
        base.Awake();
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
    }

    private void Update()
    {
        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

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
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        inGameMenuManager.EnableInGameMenu(inGameMenu);
    }

    public void DisableInGameMenu(string inGameMenu)
    {
        if (ScreenFadeManager.Instance.IsFading || LoadingScreenManager.Instance.IsVisible)
        {
            return;
        }

        inGameMenuManager.DisableInGameMenu(inGameMenu);
    }

    public void RestartCheckpoint()
    {
        GameManager.Instance.UnpauseGame();
        InputManager.Instance.DisablePlayerActions();
        SceneLoader.Instance.StartLoading(LoadingType.ContinueGame);
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}