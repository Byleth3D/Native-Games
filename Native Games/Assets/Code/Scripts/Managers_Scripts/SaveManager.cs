using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public List<SaveData> SaveFiles {  get; private set; }
    public int CurrentSaveIndex { get; private set; } = -1;

    protected override void Awake()
    {
        base.Awake();
        PreloadSaveFiles();
    }

    public void PreloadSaveFiles()
    {
        SaveFiles = new List<SaveData>();

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
                    activeSceneName = PlayerPrefs.GetString($"Save_{i}_ActiveSceneName")
                };

                SaveFiles.Add(saveData);
            }
        }
    }

    public void NewSaveGame()
    {
        SaveData saveData = new SaveData()
        {
            fileName = "SaveX",
            fileDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            checkpointIndex = CheckpointManager.Instance.CurrentCheckpointIndex,
            inventoryItems = InventoryManager.Instance.SaveInventoryItems(),
            inventoryItemsAmount = InventoryManager.Instance.SaveInventoryItemsAmount(),
            collectedItems = InventoryManager.Instance.SaveCollectedItems(),
            activeSceneName = SceneLoader.Instance.GetActiveSceneName()
        };

        CurrentSaveIndex = SaveGame(saveData);
    }

    public void NewSaveGameFromMenu()
    {
        SaveData saveData = new SaveData()
        {
            fileName = "SaveX",
            fileDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            checkpointIndex = 0,
            inventoryItems = "",
            inventoryItemsAmount = "",
            collectedItems = "",
            activeSceneName = SceneLoader.Instance.GetNextSceneName()
        };

        CurrentSaveIndex = SaveGame(saveData);
    }

    private int SaveGame(SaveData saveData)
    {
        int saveIndex = 0;

        if (!PlayerPrefs.HasKey("SaveDataCount"))
        {
            PlayerPrefs.SetInt("SaveDataCount", 0);
        }

        saveIndex = PlayerPrefs.GetInt("SaveDataCount");
        PlayerPrefs.SetInt("SaveDataCount", saveIndex + 1);

        PlayerPrefs.SetString($"Save_{saveIndex}_FileName", saveData.fileName);
        PlayerPrefs.SetString($"Save_{saveIndex}_FileDate", saveData.fileDate);
        PlayerPrefs.SetInt($"Save_{saveIndex}_CheckpointIndex", saveData.checkpointIndex);
        PlayerPrefs.SetString($"Save_{saveIndex}_InventoryItems", saveData.inventoryItems);
        PlayerPrefs.SetString($"Save_{saveIndex}_InventoryItemsAmount", saveData.inventoryItemsAmount);
        PlayerPrefs.SetString($"Save_{saveIndex}_CollectedItems", saveData.collectedItems);
        PlayerPrefs.SetString($"Save_{saveIndex}_ActiveSceneName", saveData.activeSceneName);

        SaveFiles.Add(saveData);
        return saveIndex;
    }

    public void LoadGame(int index)
    {
        if (index >= SaveFiles.Count)
        {
            Debug.LogError("Array Out of Bounds");
            Debug.Log(SaveFiles.Count);
            return;
        }

        SaveData saveFile = SaveFiles[index];
        CurrentSaveIndex = index;

        CheckpointManager.Instance.ReloadCheckpointsFrom(saveFile.checkpointIndex);
        InventoryManager.Instance.LoadInventory(saveFile.inventoryItems, saveFile.inventoryItemsAmount);
        InventoryManager.Instance.LoadCollectedItems(saveFile.collectedItems);
    }

    public void LoadLastGame()
    {
        if (!PlayerPrefs.HasKey("SaveDataCount"))
        {
            Debug.Log("No SaveData Found!");
            return;
        }

        int index = PlayerPrefs.GetInt("SaveDataCount") - 1;
        LoadGame(index);
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

    public string activeSceneName;
}