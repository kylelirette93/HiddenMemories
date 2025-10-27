using System;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public InteractionType type;

    // Reference to scriptable object associated with this interactable.
    public ItemDataSO itemData;

    public void Interact()
    {
        switch (type)
        {
            case InteractionType.Pickup:
                if (itemData.itemType == ItemType.Weapon)
                {
                    InteractableActions.AddWeapon?.Invoke(itemData);
                }
                else if (itemData.itemType == ItemType.HealthPotion)
                {
                    InteractableActions.AddPotion?.Invoke(itemData);
                }
                else if (itemData.itemType == ItemType.Ammo)
                {
                    // Add ammo.
                }
                else if (itemData.itemType == ItemType.Cash)
                {
                    // Add cash.
                    InteractableActions.AddCash?.Invoke(itemData);
                }
                else if (itemData.itemType == ItemType.Key)
                {
                    InteractableActions.AddKey?.Invoke(itemData);
                }
                Destroy(gameObject);
                break;
            case InteractionType.Door:
                Door door = GetComponent<Door>();
                door.TryUnlock();
                break;
        }
    }
}

public enum InteractionType
{
    Pickup,
    Door
}

public static class InteractableActions
{
    // Action when an item is picked up.
    public static Action<ItemDataSO> AddWeapon;
    public static Action<ItemDataSO> AddCash;
    public static Action<ItemDataSO> AddKey;
    public static Action<ItemDataSO> AddPotion;
} 


