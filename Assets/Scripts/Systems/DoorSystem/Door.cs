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

    void Start()
    {
        transform.rotation = closedRotation;
        targetRotation = closedRotation;
    }

    public void LoadData(GameData data)
    {
        // If door number is valid, set door state.
        if (doorNumber >= 0 && doorNumber < data.doorsOpened.Count)
        {
            bool shouldBeOpen = data.doorsOpened[doorNumber];

            if (shouldBeOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        // If door number invalid, door is closed by default.
        else
        {
            Close();
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
        PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        bool hasKey = inventory.Keys.Any(key => key.itemName == keyToUnlock.itemName);
        if (hasKey && !isOpen)
        {
            GameManager.Instance.audioManager.PlaySound("door_open");
            GameManager.Instance.hud.DisplayPrompt("Door unlocked", new Vector2(0, 100));
            Open();
            inventory.RemoveKey(keyToUnlock);

            GameManager.Instance.dataPersistenceManager.SaveGame();
        }
        else if (!hasKey && isOpen) 
        {
            Open();
        }
        else
        {
            GameManager.Instance.audioManager.PlaySound("door_locked");
            GameManager.Instance.hud.DisplayPrompt("You need a key to unlock this door", new Vector2(0, 100));
        }
    }

    public void Close()
    {
        if (isOpen) 
        {
            if (obstacle != null) obstacle.enabled = true;
            transform.rotation = closedRotation;
            targetRotation = closedRotation;
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (obstacle != null) obstacle.enabled = false;
            targetRotation = openRotation;
        }
    }

    public bool IsUnlocked()
    {
        if (isOpen) return true;
        else return false;
    }

    private void Update()
    {
        if (obstacle == null) return;
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            obstacle.enabled = false;
        }
        else
        {
            obstacle.enabled = true;
            targetRotation = closedRotation;
        }
    }
}