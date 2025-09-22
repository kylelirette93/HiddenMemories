using System;
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
                    // Add weapon of type.
                }
                else if (itemData.itemType == ItemType.Ammo)
                {
                    // Add ammo.
                }
                    break;
        }
    }
}

public enum InteractionType
{
    Pickup
}

public static class InteractableActions
{
    // Action when an item is picked up.
    public static Action<ItemDataSO> AddWeapon;
} 


