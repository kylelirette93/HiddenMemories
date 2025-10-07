using UnityEngine;

public class Door : MonoBehaviour
{
    public KeyDataSO keyToUnlock;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = other.gameObject.GetComponent<PlayerInventory>();
            if (inventory.Keys.Contains(keyToUnlock))
            {
                Destroy(gameObject);
            }
        }
    }
}
