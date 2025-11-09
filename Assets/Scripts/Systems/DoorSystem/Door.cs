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
        if (doorNumber >= 0 && doorNumber < data.doorsOpened.Count)
        {
            bool shouldBeOpen = data.doorsOpened[doorNumber];

            if (shouldBeOpen)
            {
                isOpen = true;
                if (obstacle != null) obstacle.enabled = false;
                transform.rotation = openRotation;
                targetRotation = openRotation;
            }
            else
            {
                isOpen = false;
                if (obstacle != null) obstacle.enabled = true;
                transform.rotation = closedRotation;
                targetRotation = closedRotation;
            }
        }
        else
        {
            isOpen = false;
            if (obstacle != null) obstacle.enabled = true;
            transform.rotation = closedRotation;
            targetRotation = closedRotation;
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
            isOpen = true;
            if (obstacle != null) obstacle.enabled = false;
            targetRotation = openRotation;
            inventory.RemoveKey(keyToUnlock);

            GameManager.Instance.dataPersistenceManager.SaveGame();
        }
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