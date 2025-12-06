using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    // Parent object for whatever weapon the player is holding.
    [SerializeField] Transform weaponParent;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] InputManager inputManager;

    private int currentWeaponIndex = -1;
    private GameObject currentWeaponInstance;
    private WeaponDataSO weaponData;
    private WeaponBase equippedWeapon;
    private WeaponUI weaponUI;
    private Dictionary<int, GameObject> weaponInstances = new Dictionary<int, GameObject>();
    AmmoManager ammoManager = new AmmoManager();

    public WeaponBase EquippedWeapon => equippedWeapon;

    private void Awake()
    {
        weaponUI = FindFirstObjectByType<WeaponUI>();
    }

    private void Start()
    {
        inputManager.ScrollWeaponEvent += SwitchWeapon;
    }

    private void Update()
    {
        if (equippedWeapon != null && weaponUI != null && weaponData != null)
        {
            UpdateUI();
        }
    }

    public void SwitchWeapon(Vector2 scrollInput)
    {
        if (inventory?.AvailableWeapons?.Count == 0) return;

        int newIndex = currentWeaponIndex;
        if (scrollInput.y > 0) newIndex++;
        else if (scrollInput.y < 0) newIndex--;
        else return;

        newIndex = (newIndex + inventory.AvailableWeapons.Count) % inventory.AvailableWeapons.Count;
        if (newIndex != currentWeaponIndex) EquipWeaponByIndex(newIndex);
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        EquipWeaponByIndex(weaponData.index);
    }

    public void EquipWeaponByIndex(int index)
    {
        // Sanity checks.
        if (inventory?.AvailableWeapons == null || index < 0 || index >= inventory.AvailableWeapons.Count) return;
        if (equippedWeapon?.IsReloading == true || index == currentWeaponIndex) return;
        // Save ammo before switch.
        if (equippedWeapon != null) ammoManager.Save(weaponData.index, equippedWeapon.CurrentAmmo);

        // Refactored to enable instead of destruction.
        if (currentWeaponInstance != null) currentWeaponInstance.SetActive(false);

        WeaponDataSO weaponToEquip = inventory.AvailableWeapons[index];

        if (!weaponInstances.TryGetValue(index, out currentWeaponInstance))
        {
            currentWeaponInstance = Instantiate(weaponToEquip.weaponPrefab, weaponParent);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstances[index] = currentWeaponInstance;
        }
        // Active and initialize weapon with data.
        currentWeaponInstance.SetActive(true);
        equippedWeapon = currentWeaponInstance.GetComponent<WeaponBase>();
        equippedWeapon.Initialize(weaponToEquip);
        equippedWeapon.CurrentAmmo = ammoManager.Load(index, weaponToEquip.ClipCapacity);
        weaponData = weaponToEquip;
        currentWeaponIndex = index;
    }
    public void UpdateUI()
    {
        if (equippedWeapon == null || weaponUI == null) return;
        weaponUI.UpdateWeaponInfo(weaponData, equippedWeapon);
    }
}