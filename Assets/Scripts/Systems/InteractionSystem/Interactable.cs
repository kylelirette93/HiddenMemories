using System;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : BaseInteractable
{
    public InteractionType type;

    // Reference to scriptable object associated with this interactable.
    public ItemDataSO itemData;

    private void Start()
    {
        if (type == InteractionType.Door)
        {
            PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            Door door = GetComponent<Door>();
            if (playerInventory != null && door != null)
            {
                interactionPromptText = playerInventory.Keys.Contains(door.keyToUnlock) ? "Press E to Open" : "No key found in Inventory...";
            }
        }
    }

    public override void OnInteract()
    {
        switch (type)
        {
            case InteractionType.Pickup:
                if (itemData.itemType == ItemType.Weapon)
                {
                    InteractableActions.AddWeapon?.Invoke(itemData);
                    GameManager.Instance.audioManager.PlaySound("key_pickup");
                }
                else if (itemData.itemType == ItemType.HealthPotion)
                {
                    InteractableActions.AddPotion?.Invoke(itemData);
                    GameManager.Instance.audioManager.PlaySound("key_pickup");
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
                    GameManager.Instance.audioManager.PlaySound("key_pickup");
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


