using System.Net;
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
            // this is a health upgrade.
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
        upgrade.Purchase();
        UpdateUI();
    }

    public void UpdateUI()
    {
        int currentTier = GameManager.Instance.upgradeManager.GetUpgradeTier(upgrade);

        if (nameText != null)
        {
            nameText.text = upgrade.upgradeType.ToString();
        }

        if (currentTier >= upgrade.MaxTier)
        {
            costText.text = "MAX";
            button.interactable = false;
        }
        else
        {
            int cost = upgrade.GetCost(currentTier);
            costText.text = cost.ToString();
            button.interactable = GameManager.Instance.currencyManager.Currency >= cost;
        }
    }
}
