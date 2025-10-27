using System.Linq;
using UnityEngine;

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
                transform.rotation = openRotation;
                targetRotation = openRotation;
            }
            else
            {
                // The saved state is CLOSED
                isOpen = false;
                // Set rotation immediately to the final closed position
                transform.rotation = closedRotation;
                targetRotation = closedRotation;
            }
        }
        else
        {
            isOpen = false;
            transform.rotation = closedRotation;
            targetRotation = closedRotation;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (isOpen)
            data.doorsOpened[doorNumber] = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            bool hasKey = inventory.Keys.Any(key => key.itemName == keyToUnlock.itemName);
            if (hasKey && !isOpen)
            {
                GameManager.Instance.uiManager.hud.InitiatePopup("Door opened with key");
                isOpen = true;
                targetRotation = openRotation;
            }
        }
    }

    private void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            targetRotation = closedRotation;
        }
    }
}