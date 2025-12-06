using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Base interface for interaction strategies.
/// </summary>
public interface IInteractionStrategy
{
    void Interact(Interactable context);

    string Prompt(Interactable context);
}

/// <summary>
/// Strategy for interacting with doors.
/// </summary>
public class DoorInteractionStrategy : IInteractionStrategy
{
    public void Interact(Interactable context)
    {
        Door door = context.GetComponent<Door>();
        if (door != null)
        {
            door.TryUnlock();
        }
    }

    public string Prompt(Interactable context)
    {
        Door door = context.GetComponent<Door>();
        if (door == null || PlayerInventory.Instance == null) return string.Empty;
        if (door.isOpen)
        {
            context.CanInteract = false;
            return string.Empty;
        }
        else if (context.CanInteract)
        {
            return PlayerInventory.Instance.Keys.Contains(door.keyToUnlock)
                ? "Press E to Open"
                : "No key found in Inventory...";
        }
        return string.Empty;
    }
}

/// <summary>
/// Strategy for picking up items.
/// </summary>
public class PickupInteractionStrategy : IInteractionStrategy
{
    private readonly Dictionary<ItemType, Action<ItemDataSO>> pickupActions = new Dictionary<ItemType, Action<ItemDataSO>>()
    {
        { ItemType.Weapon, InteractableActions.AddWeapon },
        { ItemType.HealthPotion, InteractableActions.AddPotion },
        { ItemType.Key, InteractableActions.AddKey },
        { ItemType.Cash, InteractableActions.AddCash }
    };
    public void Interact(Interactable context)
    {
        ItemDataSO item = context.itemData;

        if (pickupActions.TryGetValue(item.itemType, out Action<ItemDataSO> action))
        {
            action?.Invoke(item);
        }
        else
        {
            Debug.Log("No action defined for item type: " + item.itemType);
        }

        // After pickup, clear focus.
        context.SetFocus(false);
        UnityEngine.Object.Destroy(context.gameObject);
    }

    public string Prompt(Interactable context)
    {
        return "Press E to Pick Up " + context.itemData.itemName;
    }
}
