using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public KeyDataSO keyToUnlock;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            bool hasKey = inventory.Keys.Any(key => key.itemName == keyToUnlock.itemName);
            if (hasKey)
            {
                GameManager.Instance.uiManager.hud.InitiatePopup("Door opened with key");
                Destroy(gameObject);
            }
        }
    }
}
