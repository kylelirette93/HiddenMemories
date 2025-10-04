using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI ammoText;

    public void UpdateWeaponInfo(WeaponDataSO weaponData, WeaponBase weaponInstance)
    {
        if (weaponData == null || weaponInstance == null) return;
        icon.enabled = true;
        ammoText.enabled = true;
        icon.sprite = weaponData.icon;
        ammoText.text = $"{weaponInstance.CurrentAmmo}/{weaponInstance.ClipCapacity}";
    }
}
