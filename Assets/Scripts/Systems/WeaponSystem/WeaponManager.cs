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
    public InputManager inputManager;

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
        if (inventory == null || inventory.availableWeapons.Count == 0)
        {
            Debug.LogWarning("No weapons available in inventory.");
            return;
        }

        int newIndex = currentWeaponIndex;
        if (scrollInput.y > 0)
        {
            newIndex++;
        }
        else if (scrollInput.y < 0)
        {
            newIndex--;
        }
        else
        {
            Debug.Log("NO SCROLL detected (scrollInput.y == 0)");
        }

        if (newIndex < 0) newIndex = inventory.availableWeapons.Count - 1;
        else if (newIndex >= inventory.availableWeapons.Count) newIndex = 0;

        if (newIndex == currentWeaponIndex)
        {
            return;
        }

        EquipWeaponByIndex(newIndex);
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        EquipWeaponByIndex(weaponData.index);
    }

    public void EquipWeaponByIndex(int index)
    {
        if (equippedWeapon != null && equippedWeapon.IsReloading) return;
        if (inventory == null || inventory.availableWeapons.Count == 0)
        {
            Debug.LogWarning("No weapons available in inventory.");
            return;
        }

        if (index < 0 || index >= inventory.availableWeapons.Count)
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
            return;
        }

        if (index == currentWeaponIndex) return;

        if (currentWeaponInstance != null) Destroy(currentWeaponInstance);

        WeaponDataSO weaponToEquip = inventory.availableWeapons[index];
        currentWeaponInstance = Instantiate(weaponToEquip.weaponPrefab, weaponParent);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
        currentWeaponIndex = index;


        WeaponBase weaponBase = currentWeaponInstance.GetComponent<WeaponBase>();
        if (weaponBase != null)
        {
            weaponBase.Initialize(weaponToEquip);
            equippedWeapon = weaponBase;
            weaponData = weaponToEquip;
        }
        else
        {
            Debug.LogWarning("Weapon prefab does not have a WeaponBase component.");
        }
    }

    public void UpdateUI()
    {
        if (equippedWeapon == null || weaponUI == null) return;
        weaponUI.UpdateWeaponInfo(weaponData, equippedWeapon);
    }
}
