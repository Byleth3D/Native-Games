using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public List<SaveData> Saves { get; private set; }
    public int CurrentSaveIndex { get; private set; } = -1;
    public event Action<SaveData> OnDataLoaded;
    public event Action OnDataClear;

    protected override void Awake()
    {
        base.Awake();
        Saves = new List<SaveData>();
        PreloadSaves();
    }

    public void LoadGame(int index)
    {
        if (index >= Saves.Count)
        {
            Debug.LogError($"Save index returns 'Array Out of Bounds'; index: {index}");
            return;
        }

        SaveData save = Saves[index];
        CurrentSaveIndex = index;

        OnDataLoaded?.Invoke(save);
    }

    public void LoadGame()
    {
        if (Saves == null || Saves.Count == 0)
        {
            Debug.LogError("No Saves Found!");
            return;
        }

        int index = Saves.Count - 1;
        LoadGame(index);
    }

    public void PreloadSaves()
    {
        if (PlayerPrefs.HasKey("SaveDataCount"))
        {
            int saveDataCount = PlayerPrefs.GetInt("SaveDataCount");

            for (int i = 0; i < saveDataCount; i++)
            {
                SaveData saveData = new SaveData()
                {
                    fileName = PlayerPrefs.GetString($"Save_{i}_FileName"),
                    fileDate = PlayerPrefs.GetString($"Save_{i}_FileDate"),
                    checkpointIndex = PlayerPrefs.GetInt($"Save_{i}_CheckpointIndex"),
                    inventoryItems = PlayerPrefs.GetString($"Save_{i}_InventoryItems"),
                    inventoryItemsAmount = PlayerPrefs.GetString($"Save_{i}_InventoryItemsAmount"),
                    collectedItems = PlayerPrefs.GetString($"Save_{i}_CollectedItems"),
                    playedDialogues = PlayerPrefs.GetString($"Save_{i}_PlayedDialogues"),
                    activeSceneName = PlayerPrefs.GetString($"Save_{i}_ActiveSceneName")
                };

                Saves.Add(saveData);
            }
        }
    }

    public void CreateSaveGame(bool fromCheckpoint, bool onActiveScene, string sceneToLoad = "")
    {
        string sceneName = "";

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneName = onActiveScene ?
            SceneManagerWrapper.GetActiveSceneName() : SceneManagerWrapper.GetNextSceneName();
        }
        else if (!onActiveScene)
        {
            sceneName = sceneToLoad;
        }

        SaveData save = null;

        if (!fromCheckpoint)
        {
            save = new SaveData()
            {
                fileName = "SaveX",
                fileDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                checkpointIndex = 0,
                inventoryItems = "",
                inventoryItemsAmount = "",
                collectedItems = "",
                playedDialogues = "",
                activeSceneName = sceneName
            };
        }
        else
        {
            save = new SaveData()
            {
                fileName = "SaveX",
                fileDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                checkpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex,
                inventoryItems = InventoryManager.Instance.SaveInventoryItems(),
                inventoryItemsAmount = InventoryManager.Instance.SaveInventoryItemsAmount(),
                collectedItems = ObjectStateManager.Instance.SaveCollectedItems(),
                playedDialogues = ObjectStateManager.Instance.SavePlayedDialogues(),
                activeSceneName = sceneName
            };
        }


        CurrentSaveIndex = SaveGame(save);
        Saves.Add(save);
    }

    private int SaveGame(SaveData save)
    {
        int saveIndex = 0;

        if (!PlayerPrefs.HasKey("SaveDataCount"))
        {
            PlayerPrefs.SetInt("SaveDataCount", 0);
        }

        saveIndex = PlayerPrefs.GetInt("SaveDataCount");
        PlayerPrefs.SetInt("SaveDataCount", saveIndex + 1);

        PlayerPrefs.SetString($"Save_{saveIndex}_FileName", save.fileName);
        PlayerPrefs.SetString($"Save_{saveIndex}_FileDate", save.fileDate);
        PlayerPrefs.SetInt($"Save_{saveIndex}_CheckpointIndex", save.checkpointIndex);
        PlayerPrefs.SetString($"Save_{saveIndex}_InventoryItems", save.inventoryItems);
        PlayerPrefs.SetString($"Save_{saveIndex}_InventoryItemsAmount", save.inventoryItemsAmount);
        PlayerPrefs.SetString($"Save_{saveIndex}_CollectedItems", save.collectedItems);
        PlayerPrefs.SetString($"Save_{saveIndex}_PlayedDialogues", save.playedDialogues);
        PlayerPrefs.SetString($"Save_{saveIndex}_ActiveSceneName", save.activeSceneName);

        return saveIndex;
    }

    public SaveData GetCurrentSave()
    {
        return Saves[CurrentSaveIndex];
    }

    public void ClearLoadedData()
    {
        CurrentSaveIndex = -1;
        OnDataClear?.Invoke();
    }

    public SaveData GetSaveForLoading(LoadingType loadingType, int saveIndex)
    {
        SaveData save = null;

        switch (loadingType)
        {
            case LoadingType.NewGame:
                CreateSaveGame(fromCheckpoint: false, onActiveScene: false);
                break;

            case LoadingType.ContinueGame:
                save = Saves[Saves.Count - 1];
                break;

            case LoadingType.LoadGame:
                save = Saves[saveIndex];
                break;

            case LoadingType.NextScene:
                string nextSceneName = SceneManagerWrapper.GetNextSceneName();

                if (nextSceneName.Contains("Level"))
                {
                    goto case LoadingType.NewGame;
                }

                break;

            case LoadingType.RestartCheckpoint:
                save = Saves[CurrentSaveIndex];
                break;
        }

        return save;
    }

    public int GetSaveIndex(SaveData save)
    {
        if (save == null || !Saves.Contains(save))
        {
            return -1;
        }

        return Saves.IndexOf(save);
    }
}

public class SaveData
{
    public string fileName;
    public string fileDate;

    public int checkpointIndex;

    public string inventoryItems;
    public string inventoryItemsAmount;

    public string collectedItems;
    public string playedDialogues;

    public string activeSceneName;
}