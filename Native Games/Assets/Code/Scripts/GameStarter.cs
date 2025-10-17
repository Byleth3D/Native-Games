using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStarter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void SetupGame()
    {
        SceneManager.LoadSceneAsync("GlobalManagers", LoadSceneMode.Additive);
    }
}
