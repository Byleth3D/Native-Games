using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private List<SaveData> saveFiles;
    private int currentSaveIndex = -1;

    protected override void Awake()
    {
        base.Awake();
        PreloadSaveFiles();
        LoadGame(0);
    }

    public void PreloadSaveFiles()
    {
        saveFiles = new List<SaveData>();

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
                    inventoryItemsAmount = PlayerPrefs.GetString($"Save_{i}_InventoryItemsAmount")
                };

                saveFiles.Add(saveData);
            }
        }
    }

    public void SaveGame(SaveData saveData)
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

        saveFiles.Add(saveData);
    }

    public void LoadGame(int index)
    {
        if (index >= saveFiles.Count)
        {
            Debug.LogError("Array Out of Bounds");
            Debug.Log(saveFiles.Count);
            return;
        }

        SaveData saveFile = saveFiles[index];
        currentSaveIndex = index;

        CheckpointManager.Instance.CheckpointTeleport(saveFile.checkpointIndex);
        InventoryManager.Instance.LoadInventory(saveFile.inventoryItems, saveFile.inventoryItemsAmount);
    }
}

public class SaveData
{
    public string fileName;
    public string fileDate;

    public int checkpointIndex;

    public string inventoryItems;
    public string inventoryItemsAmount;
}