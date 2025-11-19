using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeDataSO Upgrade => upgrade;
    [SerializeField] private UpgradeDataSO upgrade;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    [SerializeField] private List<Image> tierIcons = new List<Image>();
    
    public WeaponDataSO weapon;
    UpgradeManager upgradeManager;
    bool isInitialized = false;
    private void OnEnable()
    {
        // Prevent adding numerous buttons.
        if (isInitialized) return;
        upgradeManager = GameManager.Instance.upgradeManager;
        if (weapon == null)
        {
            // This is a health upgrade.
            button.onClick.AddListener(OnPurchaseClicked);
            if (!upgradeManager.upgradeButtons.Contains(this))
            {
                upgradeManager.AddButton(this);
            }
            isInitialized = true;
        }
        else
        {
            foreach (var weapon in upgradeManager.unlockedWeapons)
            {
                if (weapon.AvailableUpgrades.Contains(upgrade))
                {
                    this.weapon = weapon;
                    break;
                }
            }
            if (!upgradeManager.upgradeButtons.Contains(this))
            {
                upgradeManager.AddButton(this);
            }
            if (button != null && this.weapon != null && upgradeManager.unlockedWeapons.Contains(this.weapon))
            {
                button.onClick.AddListener(OnPurchaseClicked);
                isInitialized = true;
            }
            else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
            UpdateUI();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnPurchaseClicked);
        isInitialized = false;
    }

    private void OnPurchaseClicked()
    {
        button.interactable = false;
        upgrade.Purchase();
        UpdateUI();
    }

    public void UpdateUI()
    {
        int currentTier = GameManager.Instance.upgradeManager.GetUpgradeTier(upgrade);
        for (int i = 0; i < tierIcons.Count; i++)
        {
            if (i < currentTier)
            {
                tierIcons[i].color = Color.white;
            }
            else
            {
                tierIcons[i].color = Color.black;
            }
        }

        if (nameText != null)
        {
            nameText.text = upgrade.Name.ToString();
        }

        if (currentTier >= upgrade.MaxTier)
        {
            costText.text = "MAX";
            button.interactable = false;
        }
        else
        {
            int cost = upgrade.GetCost(currentTier);
            costText.text = "$" + cost.ToString();
            button.interactable = GameManager.Instance.currencyManager.Currency >= cost;
        }
    }
}
