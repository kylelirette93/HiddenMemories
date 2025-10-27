using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage config")]
    [SerializeField] private string fileName;

    private GameData gameData;

    public List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple instances of data persistence manager detected.");
        }
        instance = this;

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public bool DoesSaveDataExist()
    {
        bool doesExist = dataHandler.DoesFileExist();
        return doesExist;
    }

    public GameData GetGameData()
    {
        return this.gameData;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO: load any saved data from a file.
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }
        // Push loaded data to scripts that need it.
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // Pass data to other scripts so they can update it.
        // Save data to a file using data handler.
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            if (dataPersistenceObj != null)
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved file.");
        dataHandler.Save(gameData);
    }

    public void NewGamePlusSaveClear()
    {
        gameData.inventoryData.Clear();

        // Increase enemy stats for new game plus.
        gameData.enemyData.navSpeed += 0.5f;
        gameData.enemyData.timeBetweenAttacks -= 0.2f;
        gameData.enemyData.attackDamage += 5;
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
        playerInventory.Keys.Clear();
        gameData.doorsOpened.Clear();
        gameData.doorsOpened = new List<bool> { false, false };
    }


    private void OnApplicationQuit()
    {
        // Auto save on quit.
        if (GameManager.Instance.gameStateManager.currentState != GameState.MainMenu)
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void RegisterDataPersistenceObject(IDataPersistence obj)
    {
        if (!dataPersistenceObjects.Contains(obj))
        {
            dataPersistenceObjects.Add(obj);
        }
    }

    public void UnregisterDataPersistenceObject(IDataPersistence obj)
    {
        if (dataPersistenceObjects.Contains(obj))
        {
            dataPersistenceObjects.Remove(obj);
        }
    }
}

