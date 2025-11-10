using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Parent object for whatever weapon the player is holding.
    public Transform weaponParent;

    public PlayerInventory inventory;

    private int currentWeaponIndex = -1;
    private GameObject currentWeaponInstance;
    private WeaponDataSO weaponData;
    public WeaponBase EquippedWeapon { get { return equippedWeapon; } }
    private WeaponBase equippedWeapon;
    private WeaponUI weaponUI;
    public InputManager inputManager;
    public Dictionary<int, int> ammoCounts = new Dictionary<int, int>();

    [Header("Weapon Swapping Settings")]
    [SerializeField] float swapDuration = 0.1f;
    bool isSwapping = false;

    private void Awake()
    {   
        weaponUI = FindFirstObjectByType<WeaponUI>();
    }

    private void OnDisable()
    {
        if (currentWeaponInstance != null)
        {
            currentWeaponInstance.transform.DOKill();
        }
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
        int newIndex = currentWeaponIndex;
        if (scrollInput.y > 0)
        {
            newIndex++;
        }
        else if (scrollInput.y < 0)
        {
            newIndex--;
        }

        if (newIndex < 0) newIndex = inventory.availableWeapons.Count - 1;
        else if (newIndex >= inventory.availableWeapons.Count) newIndex = 0;

        EquipWeaponByIndex(newIndex);
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        EquipWeaponByIndex(weaponData.index);
    }

    public void EquipWeaponByIndex(int index)
    {
        if (isSwapping) return;
        if (equippedWeapon != null && equippedWeapon.IsReloading) return;
        if (inventory == null || inventory.availableWeapons.Count == 0) return;
        if (index < 0 || index >= inventory.availableWeapons.Count) return;
        if (index == currentWeaponIndex) return;

        if (weaponData != null && equippedWeapon != null)
        {
            ammoCounts[weaponData.index] = equippedWeapon.CurrentAmmo;
        }

        isSwapping = true;

        // Animate weapon swap.
        if (currentWeaponInstance != null)
        {
            Sequence weaponSwap = DOTween.Sequence();
            weaponSwap.Append(currentWeaponInstance.transform.DOLocalMoveY(-1f, swapDuration).SetEase(Ease.InBack));
            weaponSwap.Append(currentWeaponInstance.transform.DOLocalMoveX(-1f, swapDuration).SetEase(Ease.InBack));
            weaponSwap.Join(currentWeaponInstance.transform.DOLocalRotate(new Vector3(-90, 90, -90), swapDuration).SetEase(Ease.InBack));
            weaponSwap.OnComplete(() => {
                    Destroy(currentWeaponInstance);
                    SpawnNewWeapon(index);
                });
           
        }
        else
        {
            SpawnNewWeapon(index);
        }
    }


    private void SpawnNewWeapon(int index)
    {
        WeaponDataSO weaponToEquip = inventory.availableWeapons[index];
        if (weaponToEquip != null && weaponParent != null)
        {
            currentWeaponInstance = Instantiate(weaponToEquip.weaponPrefab, weaponParent);
            currentWeaponInstance.transform.localPosition = new Vector3(0, -1f, 0); // Start below
            currentWeaponInstance.transform.localRotation = Quaternion.identity;
            currentWeaponIndex = index;

            Sequence weaponSwapIn = DOTween.Sequence();
            weaponSwapIn.Append(currentWeaponInstance.transform.DOLocalMoveY(0f, swapDuration));
            weaponSwapIn.Append(currentWeaponInstance.transform.DOLocalMoveX(0f, swapDuration).SetEase(Ease.InBack));
            weaponSwapIn.Join(currentWeaponInstance.transform.DOLocalRotate(Vector3.zero, swapDuration).SetEase(Ease.OutBack));
            weaponSwapIn.OnComplete(() => isSwapping = false);
        }

        WeaponBase weaponBase = currentWeaponInstance.GetComponent<WeaponBase>();
        if (weaponBase != null)
        {
            weaponBase.Initialize(weaponToEquip);
            equippedWeapon = weaponBase;
            if (ammoCounts.ContainsKey(weaponToEquip.index))
            {
                equippedWeapon.CurrentAmmo = ammoCounts[weaponToEquip.index];
            }
            weaponData = weaponToEquip;
        }
    }


    public void UpdateUI()
    {
        if (equippedWeapon == null || weaponUI == null) return;
        weaponUI.UpdateWeaponInfo(weaponData, equippedWeapon);
    }
}
