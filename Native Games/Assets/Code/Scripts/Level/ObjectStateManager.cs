using System.Collections.Generic;
using UnityEngine;

public class ObjectStateManager : LocalSingleton<ObjectStateManager>
{
    private List<GameObject> collectedItems = new List<GameObject>();
    private List<GameObject> playedDialogues = new List<GameObject>();

    private void Start()
    {
        string collectedItemsNames = SaveManager.Instance.GetCurrentSave().collectedItems;
        LoadGameObjectsToDisable(collectedItemsNames, collectedItems);

        string playedDialogueNames = SaveManager.Instance.GetCurrentSave().playedDialogues;
        LoadGameObjectsToDisable(playedDialogueNames, playedDialogues);
    }

    private void LoadGameObjectsToDisable(string gameObjcets, List<GameObject> objectList)
    {
        if (string.IsNullOrEmpty(gameObjcets))
        {
            return;
        }

        string[] gameObjectNames = gameObjcets.Split("|");

        foreach (string gameObjectName in gameObjectNames)
        {
            GameObject gameObject = GameObject.Find(gameObjectName);

            if (gameObject != null)
            {
                objectList.Add(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    public string SaveCollectedItems()
    {
        return SaveGameObjectToDisable(collectedItems);
    }

    public string SavePlayedDialogues()
    {
        return SaveGameObjectToDisable(playedDialogues);
    }

    private string SaveGameObjectToDisable(List<GameObject> objectList)
    {
        int objectListCount = objectList.Count;
        int index = 0;
        string gameObjectNames = "";

        if (objectListCount > 0)
        {
            foreach (GameObject gameObject in objectList)
            {
                if (index == objectListCount - 1)
                {
                    gameObjectNames += $"{gameObject.name}";
                }
                else
                {
                    gameObjectNames += $"{gameObject.name}|";
                    index++;
                }
            }
        }

        return gameObjectNames;
    }

    public void SetAsCollected(GameObject collectedObject)
    {
        collectedItems.Add(collectedObject);
    }

    public void SetAsPlayed(GameObject playedObject)
    {
        playedDialogues.Add(playedObject);
    }
}
