using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool Paused { get; private set; } = false;

    public void PauseGame()
    {
        InputManager.Instance.DisablePlayerActions();
        Time.timeScale = 0.0f;
        Paused = true;
    }

    public void UnpauseGame()
    {
        InputManager.Instance.EnablePlayerActions();
        Time.timeScale = 1f;
        Paused = false;
    }
}
