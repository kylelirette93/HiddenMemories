using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour, IDataPersistence
{
    private Dictionary<string, int> purchasedUpgrades = new();
    public TextMeshProUGUI currencyTXT;
    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();
    public List<WeaponDataSO> unlockedWeapons = new List<WeaponDataSO>();
    public AudioClip purchaseSound;

    private void Awake()
    {
        WeaponActions.UnlockWeapon += OnWeaponUnlocked;
    }

    private void OnDestroy()
    {
        WeaponActions.UnlockWeapon -= OnWeaponUnlocked;
    }

    private void OnWeaponUnlocked(WeaponDataSO weaponData)
    {
        // Only want to display weapons that are unlocked with upgrade UI.
        if (!unlockedWeapons.Contains(weaponData))
        {
            unlockedWeapons.Add(weaponData);
        }

        foreach (var button in upgradeButtons.ToList())
        {
            // Check if button was picked up on new run, if so the button would already be disabled.
            // So need to reenable it to update it's UI.
            if (!button.gameObject.activeInHierarchy && weaponData.AvailableUpgrades.Contains(button.Upgrade))
            {
                AddButton(button);
                button.transform.parent.gameObject.SetActive(true);
                button.weapon = weaponData;
                button.UpdateUI();
            } 
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
    }

    private void Update()
    {
        if (currencyTXT != null && currencyTXT.isActiveAndEnabled)
        {
            currencyTXT.text = GameManager.Instance.currencyManager.GetCurrency().ToString();
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
        foreach (var button in upgradeButtons)
        {
            if (button != null) button.UpdateUI();
        }
    }

    public void LoadData(GameData data)
    {
        purchasedUpgrades = new Dictionary<string, int>();

        // Convert each upgrade data into dictionary entries.
        foreach (UpgradeData upgradeData in data.purchasedUpgrades)
        {
            purchasedUpgrades[upgradeData.upgradeID] = upgradeData.tier;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.purchasedUpgrades == null)
        {
            data.purchasedUpgrades = new List<UpgradeData>();
        }
        else
        {
            data.purchasedUpgrades.Clear();
        }

        foreach (var kvp in purchasedUpgrades)
        {
            // Create new upgrade data for each entry.
            UpgradeData upgradeData = new UpgradeData();
            upgradeData.upgradeID = kvp.Key;
            upgradeData.tier = kvp.Value;
            data.purchasedUpgrades.Add(upgradeData);
        }
    }
}

[System.Serializable]
public class UpgradeData
{
    public string upgradeID;
    public int tier;
}
