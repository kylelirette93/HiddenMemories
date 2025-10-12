using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
   public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
   public WeaponManager weaponManager;
   public List<KeyDataSO> Keys = new List<KeyDataSO>();
    UIManager uiManager;

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
        }
    }
}
