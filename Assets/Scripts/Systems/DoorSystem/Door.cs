using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour, IDataPersistence
{
    public KeyDataSO keyToUnlock;
    // Keep track of door number when loading data.
    public int doorNumber;
    public bool isOpen = false;

    float rotationSpeed = 1f;
    Quaternion targetRotation;
    public Quaternion closedRotation;
    public Quaternion openRotation;

    void Start()
    {
        transform.rotation = closedRotation;
        targetRotation = closedRotation;
       
    }
    public void LoadData(GameData data)
    {
        if (data.doorsOpened.Count > 0 && data.doorsOpened[doorNumber] == true)
        {
            if (this != null)
            isOpen = true;
            targetRotation = openRotation;
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
            if (hasKey)
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
    }
}
