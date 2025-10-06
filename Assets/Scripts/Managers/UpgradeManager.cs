using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private Dictionary<string, int> purchasedUpgrades = new();
    public TextMeshProUGUI currencyTXT;
    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    public int GetUpgradeTier(UpgradeDataSO upgrade)
    {
        return purchasedUpgrades.TryGetValue(upgrade.UpgradeID, out int tier) ? tier : 0;
    }
    public void PurchaseUpgrade(UpgradeDataSO upgrade)
    {
        string id = upgrade.UpgradeID;
        purchasedUpgrades[id] = purchasedUpgrades.GetValueOrDefault(id, 0) + 1;
        GameManager.Instance.currencyManager.Currency -= upgrade.GetCost(purchasedUpgrades[id] - 1);
        UpdateAllButtons();
        // Ideally, im gonna save upgrade here wit a binary formatter.
    }

    private void Update()
    {
        if (currencyTXT != null && currencyTXT.isActiveAndEnabled)
        {
            currencyTXT.text = "Currency: " + GameManager.Instance.currencyManager.GetCurrency().ToString();
        }
    }

    public void AddButton(UpgradeButton button)
    {
        upgradeButtons.Add(button);
    }

    public void UpdateAllButtons()
    {
        foreach (UpgradeButton button in upgradeButtons)
        {
            if (button != null) button.UpdateUI();
        }
    }
}
