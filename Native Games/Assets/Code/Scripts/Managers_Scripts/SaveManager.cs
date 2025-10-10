using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private List<SaveData> saveFiles;
    private int currentSaveIndex = -1;

    public void PreloadSaveFiles()
    {
        saveFiles = new List<SaveData>();
        
    }

    public void SaveGame()
    {

    }

    public void LoadGame()
    {
        saveFiles = new List<SaveData>();
    }
}

public class SaveData
{
    public string fileName;
    public string fileDate;

    #region Checkpoint Fields
    public int checkpointIndex;

    public float checkpointPositionX;
    public float checkpointPositionY;
    public float checkpointPositionZ;

    public float checkpointForwardX;
    public float checkpointForwardY;
    public float checkpointForwardZ;
    #endregion
}

public class InventoryItemSaveData
{
    public string scriptableObjectName;
    public int currentAmount;
    public int saveDataIndex = -1;
}