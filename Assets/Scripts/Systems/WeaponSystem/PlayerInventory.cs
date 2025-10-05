using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
   public List<WeaponDataSO> availableWeapons = new List<WeaponDataSO>();
   public WeaponManager weaponManager;

    private void Awake()
    {
        InteractableActions.AddWeapon += AddWeapon;
    }

    private void OnDestroy()
    {
        InteractableActions.AddWeapon -= AddWeapon;
    }

    private void AddWeapon(ItemDataSO weapon)
    {
        if (weapon is WeaponDataSO _currentWeapon && !availableWeapons.Contains(_currentWeapon))
        {
            availableWeapons.Add(_currentWeapon);
            weaponManager.EquipWeapon(_currentWeapon);
            Debug.Log("Successfully added weapon: " + weapon.name);
        }
    }
}
