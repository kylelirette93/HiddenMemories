using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour, IDataPersistence
{
    public KeyDataSO keyToUnlock;
    // Keep track of door number when loading data.
    public int doorNumber;
    bool isOpen = false;

    public void LoadData(GameData data)
    {
        if (data.doorsOpened.Count > 0 && data.doorsOpened[doorNumber] == true)
        {
            if (this != null)
            Destroy(gameObject);
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
                Destroy(gameObject);
            }
        }
    }
}
