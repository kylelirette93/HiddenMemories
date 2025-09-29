using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Upgrades", order = 1)]
public class UpgradeDataSO : ScriptableObject
{
    // Identifier for what we're upgrading.
    public string name;
    public int id;
    public UpgradeType upgradeType;
}

public enum UpgradeType
{
    ClipCapacity,
    FireRate,
    PowerRate,
    ReloadSpeed,
    BiggerSpread,
    Recoil,
    MaxHealth
}
