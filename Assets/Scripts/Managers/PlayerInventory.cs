using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : Singleton<PlayerInventory>, IDataPersistence
{
    [Header("Item Lists")]
    List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
    List<KeyDataSO> keys = new List<KeyDataSO>();
    List<HealingPotionSO> healingPotions = new List<HealingPotionSO>();

    // Getters for item lists.
    public List<KeyDataSO> Keys => keys;
    public List<HealingPotionSO> HealingPotions => healingPotions;
    public List<WeaponDataSO> AvailableWeapons => availableWeapons;
    InputManager input;

    private readonly Dictionary<ItemType, IItemAddStrategy> addStrategies = new Dictionary<ItemType, IItemAddStrategy>();
    private readonly Dictionary<ItemType, IItemRemoveStrategy> removeStrategies = new Dictionary<ItemType, IItemRemoveStrategy>();

    // Events for updating other systems
    public Action<int> OnPotionCountChanged;
    public Action<int> OnKeyCountChanged;
    public Action<int> OnHealingPotionUsed;

    private void Awake()
    {
        // Initialize strategies for adding items.
        addStrategies.Add(ItemType.Key, new KeyAddStrategy());
        addStrategies.Add(ItemType.HealthPotion, new PotionAddStrategy());
        addStrategies.Add(ItemType.Weapon, new WeaponAddStrategy());
        removeStrategies.Add(ItemType.Key, new KeyRemoveStrategy());
        removeStrategies.Add(ItemType.HealthPotion, new PotionRemoveStrategy());

        // Player Inventory gets instantiated after, but must be saveable.
        GameManager.Instance.dataPersistenceManager.RegisterDataPersistenceObject(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.dataPersistenceManager.UnregisterDataPersistenceObject(this);
    }
    private void OnEnable()
    {
        input = GameManager.Instance.inputManager;
        InteractableActions.AddWeapon += AddItem;
        InteractableActions.AddKey += AddItem;
        InteractableActions.AddPotion += AddItem;
        input.UsePotionEvent += UseHealingPotion;
    }

    private void OnDisable()
    {
        InteractableActions.AddWeapon -= AddItem;
        InteractableActions.AddKey -= AddItem;
        InteractableActions.AddPotion -= AddItem;
        StopAllCoroutines();
    }

    /// <summary>
    /// Executes addition strategy based on item.
    /// </summary>
    /// <param name="item">The item data being passed.</param>
    private void AddItem(ItemDataSO item)
    {
        if (addStrategies.TryGetValue(item.itemType, out IItemAddStrategy strategy))
        {
            strategy.Execute(item, this);
        }
    }

    /// <summary>
    /// Executes removal strategy based on item.
    /// </summary>
    /// <param name="item">The item data being passed.</param>
    public void RemoveItem(ItemDataSO item)
    {
        if (removeStrategies.TryGetValue(item.itemType, out IItemRemoveStrategy strategy))
        {
            strategy.Execute(item, this);
        }
    }

    public void LoadData(GameData data)
    {
        availableWeapons.Clear();
        keys.Clear();
        healingPotions.Clear();

        foreach (var inventoryData in data.inventoryData)
        {
            LoadItemsFromIDs(inventoryData.weaponIDs, (id) => Resources.Load<WeaponDataSO>("ScriptableObjects/Weapons/" + id));
            LoadItemsFromIDs(inventoryData.keyIDs, (id) => Resources.Load<WeaponDataSO>("ScriptableObjects/Keys/" + id));
            LoadItemsFromCount(inventoryData.potionCount);
        }

        OnKeyCountChanged?.Invoke(keys.Count);
        OnPotionCountChanged?.Invoke(healingPotions.Count);
    }

    /// <summary>
    /// Loads items based on IDs using a provided function.
    /// </summary>
    /// <param name="itemIDs">The id from load data to use.</param>
    /// <param name="loadFunction">Calls a resource load function with path.</param>
    private void LoadItemsFromIDs(List<string> itemIDs, Func<string, ItemDataSO> loadFunction)
    {
        foreach (string itemID in itemIDs)
        {
            ItemDataSO item = loadFunction(itemID);
            if (item != null) AddItem(item);
            else Debug.LogError("Failed to load item with ID: " + itemID);
        }
    }
    /// <summary>
    /// Loads healing potions based on count. Could be more generic.
    /// </summary>
    /// <param name="count">The count of items.</param>
    private void LoadItemsFromCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            HealingPotionSO potion = Resources.Load<HealingPotionSO>("ScriptableObjects/Items/HealingPotion");
            if (potion != null) AddItem(potion);
            else Debug.LogError("Failed to load HealingPotionSO from Resources.");
        }
    }

    public void SaveData(ref GameData data)
    {
        //Debug.Log("PlayerInventory.SaveData() called! Available weapons: " + availableWeapons.Count);
        data.inventoryData.Clear();

        InventoryData saveState = new InventoryData
        {
            weaponIDs = availableWeapons
            .Where(weapon => weapon.IsUnlocked)
            .Select(weapon => weapon.name)
            .ToList(),

            keyIDs = keys.Select(key => key.name).ToList(),

            potionCount = healingPotions.Count
        };

        data.inventoryData.Add(saveState);
    } 

    public void UseHealingPotion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (healingPotions.Count > 0)
            {
                HealingPotionSO potion = healingPotions[0];
                // Decoupled from player health now...
                OnHealingPotionUsed?.Invoke(potion.HealAmount);
                RemoveItem(potion);
                GameManager.Instance.audioManager.PlaySound("heal");
            }
        }
    }
}


[System.Serializable]
public class InventoryData
{
    public List<string> weaponIDs;
    public List<string> keyIDs;
    public int potionCount;
}
