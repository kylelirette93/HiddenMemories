using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI ammoText;
    WeaponBase weapon;
    WeaponDataSO data;

    public void UpdateWeaponInfo(WeaponDataSO weaponData, WeaponBase weaponInstance)
    {
        if (weaponData == null || weaponInstance == null) 
        { 
            icon.enabled = false; return; 
        }

        data = weaponData;
        weapon = weaponInstance;

        icon.enabled = true;
        ammoText.enabled = true;
        icon.sprite = data.icon;
        ammoText.text = $"{weapon.CurrentAmmo}/{weapon.ClipCapacity}";
    }
    private void OnDisable()
    {
        icon.enabled = false;
        icon.sprite = null;
        ammoText.text = "";
    }

    private void OnEnable()
    {
        if (data == null || weapon == null) 
        { 
            icon.enabled = false; return;
        }

        icon.enabled = true;
        icon.sprite = data.icon;
        ammoText.text = $"{weapon.CurrentAmmo}/{weapon.ClipCapacity}";
    }
}
