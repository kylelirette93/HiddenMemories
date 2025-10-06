using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeDataSO upgrade;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnPurchaseClicked);
            GameManager.Instance.upgradeManager.AddButton(this);
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
