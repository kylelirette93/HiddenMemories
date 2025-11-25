using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, IDataPersistence
{
    public KeyDataSO keyToUnlock;
    // Keep track of door number when loading data.
    public int doorNumber;
    public bool isOpen = false;
    Quaternion targetRotation;
    public Quaternion closedRotation;
    public Quaternion openRotation;
    float rotationSpeed = 2.5f;
    public AudioClip doorSound;
    public NavMeshObstacle obstacle;
    InteractionManager interactionManager;
    AudioManager AudioManager => GameManager.Instance.audioManager;
    HUD HUD => GameManager.Instance.hud;
    DataPersistenceManager DataPersistenceManager => GameManager.Instance.dataPersistenceManager;

    void Start()
    {
        SetDoorState(false);
    }

    public void LoadData(GameData data)
    {
        // If door number is valid, set door state.
        if (doorNumber >= 0 && doorNumber < data.doorsOpened.Count)
        {
            SetDoorState(data.doorsOpened[doorNumber]);
        }
        else
        {
            SetDoorState(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        while (data.doorsOpened.Count <= doorNumber)
        {
            data.doorsOpened.Add(false);
        }
        data.doorsOpened[doorNumber] = isOpen;
    }

    public void TryUnlock()
    {
        if (isOpen) return; // Door is already unlocked.
        PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        if (inventory == null) return;
        bool hasKey = inventory.Keys.Any(key => key.itemName == keyToUnlock.itemName);
        if (hasKey)
        {
            // If player has key, unlock door.
            AudioManager.PlaySound("door_open");
            HUD.DisplayPrompt("Door unlocked.", new Vector2(0, 100));
            SetDoorState(true);
            inventory.RemoveKey(keyToUnlock);
            DataPersistenceManager.SaveGame();
        }
        else
        {
            // If player doesn't have key, give feedback.
            AudioManager.PlaySound("door_locked");
            HUD.DisplayPrompt("Door is locked. No key found...", new Vector2(0, 100));
        }
    }

    public void SetDoorState(bool open)
    {
        isOpen = open;
        targetRotation = open ? openRotation : closedRotation;
        if (obstacle != null) obstacle.enabled = !open;
    }

    public bool IsUnlocked()
    {
        return isOpen;
    }

    private void Update()
    {
        if (isOpen && transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (!isOpen)
        {
            transform.rotation = closedRotation;
        }
    }
}