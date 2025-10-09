using UnityEngine;

public class Door : MonoBehaviour
{
    public KeyDataSO keyToUnlock;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory.Keys.Contains(keyToUnlock))
            {
                GameManager.Instance.uiManager.hud.InitiatePopup("Door opened with key");
                Destroy(gameObject);
            }
        }
    }
}
