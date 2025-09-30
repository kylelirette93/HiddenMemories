using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Parent object for whatever weapon the player is holding.
    public Transform weaponParent;

    public PlayerInventory inventory;

    private int currentWeaponIndex = -1;
    private GameObject currentWeaponInstance;
    private WeaponDataSO weaponData;
    private WeaponBase equippedWeapon;
    private WeaponUI weaponUI;

    private void Awake()
    {
        weaponUI = FindFirstObjectByType<WeaponUI>();
    }

    private void Update()
    {
        if (weaponUI != null)
        {
            UpdateUI();
        } 
    }

    public void SwitchWeapon(int newIndex)
    {
        if (inventory.availableWeapons.Count == 0)
        {
            Debug.LogWarning("No weapons available in inventory.");
            return;
        }

        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        WeaponDataSO weaponToEquip = inventory.availableWeapons[newIndex];
        currentWeaponInstance = Instantiate(weaponToEquip.weaponPrefab, weaponParent);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
        currentWeaponIndex = newIndex;

        WeaponBase weaponBase = currentWeaponInstance.GetComponent<WeaponBase>();
        if (weaponBase != null)
        {
            weaponBase.Initialize(weaponToEquip);
            equippedWeapon = weaponBase;
        }
        else
        {
            Debug.LogWarning("Weapon prefab does not have a WeaponBase component.");
        }
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        int index = weaponData.index;
        SwitchWeapon(index);
        this.weaponData = weaponData;
    }

    public void UpdateUI()
    {
        if (equippedWeapon == null || weaponUI == null) return;
        weaponUI.UpdateWeaponInfo(weaponData, equippedWeapon);
    }
}
