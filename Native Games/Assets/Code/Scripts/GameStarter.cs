using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStarter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SetupGame()
    {
        SceneManager.LoadSceneAsync("GlobalManagers", LoadSceneMode.Additive);
    }
}
