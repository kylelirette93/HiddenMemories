using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IDataPersistence
{
   public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
   public WeaponManager weaponManager;
   public List<KeyDataSO> Keys = new List<KeyDataSO>();
    UIManager uiManager;
    HUD hud;

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
        InteractableActions.AddWeapon += AddWeapon;
        InteractableActions.AddKey += AddKey;
        uiManager = GameManager.Instance.uiManager;
    }

    private void OnDisable()
    {
        InteractableActions.AddWeapon -= AddWeapon;
        InteractableActions.AddKey -= AddKey;
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

            //uiManager.hud.AddKeyToHud(); Note to Kyle. Commented out for now, until I have a key image. 
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
                Debug.Log(keyToAdd);
                if (keyToAdd != null && !Keys.Contains(keyToAdd))
                {
                    Keys.Add(keyToAdd);
                    Debug.Log("Successfully loaded key: " + keyToAdd.name);
                }
            }
        }
    }

    public string Trim(string stringToTrim)
    {
        string wordsToTrim = "(KeyDataSO)";
        return stringToTrim.Replace(wordsToTrim, "");
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("PlayerInventory.SaveData() called! Available weapons: " + availableWeapons.Count);
        data.inventoryData.Clear();

        InventoryData inventoryData = new InventoryData();
        inventoryData.weaponIDs = new List<string>();
        inventoryData.keyIDs = new List<string>();

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
            key.name = Trim(key.name);
            inventoryData.keyIDs.Add(key.name);
            Debug.Log("Added key to save: " + key.name);
        }
        data.inventoryData.Add(inventoryData);
    }
   
}

[System.Serializable]
public class InventoryData
{
    public List<string> weaponIDs;
    public List<string> keyIDs;
}
