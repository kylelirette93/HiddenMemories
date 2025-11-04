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
        GameManager.Instance.audioManager.PlaySound("CoinPickup");
        GameManager.Instance.uiManager.hud.InitiatePopup("+1", new Vector2(1200, 500), false);
        currency += itemData.value;
        GameManager.Instance.progressManager.CurrencyAdded();
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
