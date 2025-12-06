using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : Singleton<PlayerInventory>, IDataPersistence
{
    [Header("Item Lists")]
    public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
    public List<KeyDataSO> Keys = new List<KeyDataSO>();
    public List<HealingPotionSO> HealingPotions = new List<HealingPotionSO>();
    PlayerHealth playerHealth;
    InputManager input;

    public Action<int> OnPotionCountChanged;
    public Action<int> OnKeyCountChanged;
    private readonly Dictionary<ItemType, IItemAddStrategy> addStrategies = new Dictionary<ItemType, IItemAddStrategy>();
    private readonly Dictionary<ItemType, IItemRemoveStrategy> removeStrategies = new Dictionary<ItemType, IItemRemoveStrategy>();

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
        playerHealth = GetComponent<PlayerHealth>();
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

    private void AddItem(ItemDataSO item)
    {
        if (addStrategies.TryGetValue(item.itemType, out IItemAddStrategy strategy))
        {
            strategy.Execute(item, this);
        }
    }

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
        Keys.Clear();
        HealingPotions.Clear();

        foreach (var inventoryData in data.inventoryData)
        {
            LoadItemsFromIDs(inventoryData.weaponIDs, (id) => Resources.Load<WeaponDataSO>("ScriptableObjects/Weapons/" + id));
            LoadItemsFromIDs(inventoryData.keyIDs, (id) => Resources.Load<WeaponDataSO>("ScriptableObjects/Weapons/" + id));
            LoadItemsFromCount(inventoryData.potionCount);
        }

        OnKeyCountChanged?.Invoke(Keys.Count);
        OnPotionCountChanged?.Invoke(HealingPotions.Count);
    }

    private void LoadItemsFromIDs(List<string> itemIDs, Func<string, ItemDataSO> loadFunction)
    {
        foreach (string itemID in itemIDs)
        {
            ItemDataSO item = loadFunction(itemID);
            if (item != null) AddItem(item);
            else Debug.LogError("Failed to load item with ID: " + itemID);
        }
    }

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

            keyIDs = Keys.Select(key => key.name).ToList(),

            potionCount = HealingPotions.Count
        };

        data.inventoryData.Add(saveState);
    } 

    public void UseHealingPotion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (HealingPotions.Count > 0)
            {
                HealingPotionSO potion = HealingPotions[0];
                if (playerHealth != null)
                {
                    playerHealth.Heal(potion.HealAmount);
                }
                RemoveItem(potion);
                HUD.Instance.InitiatePopup("+" + potion.HealAmount, new Vector2(-60, -490), false);
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
