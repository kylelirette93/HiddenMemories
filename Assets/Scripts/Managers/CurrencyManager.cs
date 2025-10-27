using UnityEngine;

public class CurrencyManager : MonoBehaviour, IDataPersistence
{
    public int Currency { get { return currency; } set { currency = value; } }
    int currency;
    public AudioClip CashPickup;

    private void Awake()
    {
        currency = 0;
        InteractableActions.AddCash += IncrementCurrency;
    }

    private void IncrementCurrency(ItemDataSO itemData)
    {
        GameManager.Instance.audioManager.PlaySFX(CashPickup);
        GameManager.Instance.uiManager.hud.InitiatePopup("Coin Added!");
        currency += itemData.value;
    }

    public int GetCurrency() { return currency; }

    public void LoadData(GameData data)
    {
        this.currency = data.currency;
    }

    public void SaveData(ref GameData data)
    {
        data.currency = this.currency;
    }
}
