using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private Dictionary<string, int> purchasedUpgrades = new();
    public TextMeshProUGUI currencyTXT;
    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();
    public List<WeaponBase> unlockedWeapons = new List<WeaponBase>();

    private void Awake()
    {
        WeaponActions.UnlockWeapon += OnWeaponUnlocked;
    }

    private void OnDestroy()
    {
        WeaponActions.UnlockWeapon -= OnWeaponUnlocked;
    }

    private void OnWeaponUnlocked(WeaponBase weapon)
    {
        // Only want to display weapons that are unlocked with upgrade UI.
        if (!unlockedWeapons.Contains(weapon))
        {
            unlockedWeapons.Add(weapon);
        }
    }

    public int GetUpgradeTier(UpgradeDataSO upgrade)
    {
        return purchasedUpgrades.TryGetValue(upgrade.UpgradeID, out int tier) ? tier : 0;
    }
    public void PurchaseUpgrade(UpgradeDataSO upgrade)
    {
        string id = upgrade.UpgradeID;
        int currentTier = purchasedUpgrades.GetValueOrDefault(id, 0);
        int cost = upgrade.GetCost(currentTier);
        purchasedUpgrades[id] = currentTier + 1;
        GameManager.Instance.currencyManager.Currency -= cost;
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

    public void ClearUpgrades()
    {
        purchasedUpgrades.Clear();
        UpdateAllButtons();
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
