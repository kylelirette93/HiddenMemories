using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    WeaponController weaponController;

    public void ApplyUpgrade(UpgradeDataSO upgradeData)
    {
        // Get the upgrade data passed. 
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.PowerRate:
                
                // Apply power rate here.
                break;
            case UpgradeType.ClipCapacity:
                // Apply capacity here.
                break;
            case UpgradeType.MaxHealth:
                // Apply maximum health here.
                break;
            case UpgradeType.FireRate:
                // Apply fire rate here.
                break;
            case UpgradeType.Recoil:
                // Apply recoil here.
                break;
            case UpgradeType.ReloadSpeed:
                // Apply reload speed here.
                break;
            case UpgradeType.BiggerSpread:
                // Apply spread count here.
                break;
        }
    }
}
