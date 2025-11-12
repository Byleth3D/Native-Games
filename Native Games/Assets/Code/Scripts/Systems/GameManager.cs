using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; protected set; } = GameState.Running;

    public void SwitchState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Running:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0.0f;
                break;

            case GameState.Exiting:
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    Application.Quit();
                }
                else
                {
                    goto case GameState.Running;
                }
#endif
                break;

            default:
                break;
        }

        State = state;
    }
}

public enum GameState
{
    Running,
    Paused,
    Exiting
}
