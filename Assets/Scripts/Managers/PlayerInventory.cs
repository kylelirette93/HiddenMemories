using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour, IDataPersistence
{
   public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
   public WeaponManager weaponManager;
   public List<KeyDataSO> Keys = new List<KeyDataSO>();
    public List<HealingPotionSO> HealingPotions = new List<HealingPotionSO>();
    UIManager uiManager;
    HUD hud;
    PlayerHealth playerHealth;
    InputManager input;

    private void Awake()
    {
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
        InteractableActions.AddWeapon += AddWeapon;
        InteractableActions.AddKey += AddKey;
        InteractableActions.AddPotion += AddPotion;
        input.UsePotionEvent += UseHealingPotion;
        uiManager = GameManager.Instance.uiManager;
        if (uiManager != null)
        {
            hud = uiManager.hud;
        }
    }

    private void OnDisable()
    {
        InteractableActions.AddWeapon -= AddWeapon;
        InteractableActions.AddKey -= AddKey;
        InteractableActions.AddPotion -= AddPotion;
        StopAllCoroutines();
    }

    private void AddWeapon(ItemDataSO weapon)
    {
        if (weapon is WeaponDataSO _currentWeapon && !availableWeapons.Contains(_currentWeapon))
        {
            availableWeapons.Add(_currentWeapon);
            weaponManager.EquipWeapon(_currentWeapon);
            _currentWeapon.IsUnlocked = true;
            WeaponActions.UnlockWeapon?.Invoke(_currentWeapon);
            Debug.Log("Successfully added weapon: " + weapon.name);
        }
    }

    private void AddKey(ItemDataSO key)
    {
        if (key is KeyDataSO Key)
        {
            Keys.Add(Key);
            uiManager.hud.InitiatePopup("Key added to inventory!");

            uiManager.hud.AddKeyToHud(); 
        }
    }

    private void AddPotion(ItemDataSO potion)
    {
        if (potion is HealingPotionSO Potion)
        {
            HealingPotions.Add(Potion);
            hud.InitiatePopup("Press H to heal!");
        }
    }

    public void LoadData(GameData data)
    {
        Debug.Log("PlayerInventory.LoadData() called! Inventory data count: " + data.inventoryData.Count);
        foreach (var inventoryData in data.inventoryData)
        {
            foreach (string weaponID in inventoryData.weaponIDs)
            {
                WeaponDataSO weaponToUnlock = Resources.Load<WeaponDataSO>("ScriptableObjects/Weapons/" + weaponID);

                if (weaponToUnlock != null && !availableWeapons.Contains(weaponToUnlock))
                {
                    availableWeapons.Add(weaponToUnlock);
                    weaponToUnlock.IsUnlocked = true;
                    weaponManager.EquipWeapon(weaponToUnlock);
                    WeaponActions.UnlockWeapon?.Invoke(weaponToUnlock);
                    Debug.Log("Successfully loaded weapon: " + weaponToUnlock.name);
                }
            }
            foreach (string keyID in inventoryData.keyIDs)
            {
                KeyDataSO keyToAdd = Resources.Load<KeyDataSO>("ScriptableObjects/Keys/" + keyID);
                Debug.Log(keyID);
                if (keyToAdd != null && !Keys.Contains(keyToAdd))
                {
                    Keys.Add(keyToAdd);
                    Debug.Log("Successfully loaded key: " + keyToAdd.name);
                }
            }

            for (int i = 0; i < inventoryData.potionCount; i++)
            {
                HealingPotionSO potionToAdd = Resources.Load<HealingPotionSO>("ScriptableObjects/Potions/HealingPotion");
                Debug.Log(potionToAdd);
                if (potionToAdd != null)
                {
                    HealingPotions.Add(potionToAdd);
                    Debug.Log("Successfully loaded: " + inventoryData.potionCount + " potions.");
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("PlayerInventory.SaveData() called! Available weapons: " + availableWeapons.Count);
        data.inventoryData.Clear();

        InventoryData inventoryData = new InventoryData();
        inventoryData.weaponIDs = new List<string>();
        inventoryData.keyIDs = new List<string>();
        inventoryData.potionCount = 0;

        foreach (var weapon in availableWeapons)
        {
            Debug.Log($"Weapon: {weapon.name}, IsUnlocked: {weapon.IsUnlocked}");
            if (weapon.IsUnlocked)
            {
                inventoryData.weaponIDs.Add(weapon.name);
                Debug.Log("Added weapon to save: " + weapon.name);
            }
        }
        foreach (var key in Keys)
        {
            inventoryData.keyIDs.Add(key.name);
            Debug.Log("Added key to save: " + key.name);
        }

        foreach (var healthPotion in HealingPotions)
        {
            inventoryData.potionCount++;
            Debug.Log("Added " + inventoryData.potionCount + " potions to save file.");
        }
        data.inventoryData.Add(inventoryData);
    } 

    public void UseHealingPotion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (HealingPotions.Count > 0)
            {
                HealingPotionSO potion = HealingPotions[0];
                playerHealth.Heal(potion.HealAmount);
                HealingPotions.RemoveAt(0);
                hud.InitiatePopup("+" + potion.HealAmount);
            }
        }
    }

    private void Update()
    {
        if (hud != null)
        {
            hud.UpdatePotionCount(HealingPotions.Count);
            if (Keys.Count > 0)
            {
                hud.AddKeyToHud();
            }
            else
            {
                hud.RemoveKeyFromHud();
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
