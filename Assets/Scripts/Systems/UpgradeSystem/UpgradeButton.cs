using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeDataSO upgrade;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    public WeaponBase weapon;
    UpgradeManager upgradeManager;
    private void OnEnable()
    {
        upgradeManager = GameManager.Instance.upgradeManager;
        if (weapon == null)
        {
            // this is a health upgrade.
            button.onClick.AddListener(OnPurchaseClicked);
            GameManager.Instance.upgradeManager.AddButton(this);
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
            if (button != null && weapon.IsUnlocked)
            {
                button.onClick.AddListener(OnPurchaseClicked);
                GameManager.Instance.upgradeManager.AddButton(this);
            }
            else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
            UpdateUI();
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
