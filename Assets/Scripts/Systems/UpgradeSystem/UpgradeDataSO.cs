using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/UpgradeDataSO", order = 1)]
public class UpgradeDataSO : ScriptableObject, IUpgrade
{
    [SerializeField] string upgradeID;
    [SerializeField] int maxTier = 3;
    [SerializeField] public int[] costs;
    [SerializeField] public float[] statValues;
    [SerializeField] public UpgradeType upgradeType;

    public string UpgradeID => upgradeID;
    public int MaxTier => maxTier;

    public int GetCost(int tier)
    {
        if (tier < 0 || tier >= costs.Length) return int.MaxValue;
        return costs[tier];
    }
    public void Purchase()
    {
        if (GameManager.Instance.currencyManager.Currency >= GetCost(GameManager.Instance.upgradeManager.GetUpgradeTier(this)))
        {
            GameManager.Instance.upgradeManager.PurchaseUpgrade(this);
        }
        else
        {
            Debug.Log("Not enough currency to purchase upgrade: " + upgradeID);
        }
    }

    public void Upgrade(WeaponBase weapon, int tier)
    {
        if (tier <= 0 || tier > maxTier) return;

        // Calculate upgrade value
        float value = 0f;
        for (int i = 0; i < tier; i++)
        {
            value += statValues[i];
        }

        if (weapon is Pistol pistol)
        {
            switch (upgradeType)
            {
                case UpgradeType.ClipCapacity:
                    pistol.AddClipCapacity((int)value);
                    break;

                case UpgradeType.FireRate:
                    pistol.AddFireRate(value);
                    break;
            }
        }
    }
}

public enum UpgradeType
{
    ClipCapacity,
    FireRate
}
