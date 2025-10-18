using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

[DefaultExecutionOrder(-20)]
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
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public bool DoesSaveDataExist()
    {
        bool doesExist = dataHandler.DoesFileExist();
        return doesExist;
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
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved file.");
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        // Auto save on quit.
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}

