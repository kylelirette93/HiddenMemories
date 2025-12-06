using System;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>, IDataPersistence
{
    public int Currency { get { return currency; } set { currency = value; } }
    int currency;

    private void Awake()
    {
        currency = 0;
        InteractableActions.AddCash += IncrementCurrency;
    }

    public void IncrementCurrency(ItemDataSO itemData)
    {
        if (itemData is CoinDataSO coinData)
        {
            GameManager.Instance.audioManager.PlaySound("CoinPickup");
            GameManager.Instance.uiManager.hud.InitiatePopup("+1", new Vector2(1200, 500), false);
            currency += coinData.value;
            GameManager.Instance.progressManager.CurrencyAdded();
        }    }

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
