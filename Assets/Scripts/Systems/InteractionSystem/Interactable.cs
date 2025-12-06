using System;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : BaseInteractable
{
    public InteractionType type;

    // Reference to scriptable object associated with this interactable.
    public ItemDataSO itemData;

    // Strategy to interact with this interactable.
    private IInteractionStrategy strategy;
    PlayerInventory playerInventory => PlayerInventory.Instance;

    public override void Awake()
    {
        base.Awake();
        // Initialize strategy based on type.
        strategy = type switch
        {
            InteractionType.Door => new DoorInteractionStrategy(),
            InteractionType.Pickup => new PickupInteractionStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "No strategy defined for this interaction type.")
        };
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (strategy is DoorInteractionStrategy && collision.gameObject.CompareTag("Player"))
        {
            Door door = GetComponent<Door>();
            if (PlayerInventory.Instance != null)
            {
                interactionPromptText = playerInventory.Keys.Contains(door.keyToUnlock) ? "Press E to Open" : "No key found in Inventory...";
            }
        }
    }

    public override string GetInteractionPrompt()
    {
        string prompt = strategy.Prompt(this);
        if (!string.IsNullOrEmpty(prompt))
        {
            return prompt;
        }
        return base.GetInteractionPrompt();
    }

    public override void OnInteract()
    {
        strategy.Interact(this);
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


