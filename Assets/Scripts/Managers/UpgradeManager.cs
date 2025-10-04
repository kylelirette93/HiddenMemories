using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private Dictionary<string, int> purchasedUpgrades = new();

    public int GetUpgradeTier(UpgradeDataSO upgrade)
    {
        return purchasedUpgrades.TryGetValue(upgrade.UpgradeID, out int tier) ? tier : 0;
    }
    public void PurchaseUpgrade(UpgradeDataSO upgrade)
    {
        string id = upgrade.UpgradeID;
        purchasedUpgrades[id] = purchasedUpgrades.GetValueOrDefault(id, 0) + 1;
        GameManager.Instance.currencyManager.Currency -= upgrade.GetCost(purchasedUpgrades[id] - 1);
        // Ideally, im gonna save upgrade here wit a binary formatter.
    }
}
