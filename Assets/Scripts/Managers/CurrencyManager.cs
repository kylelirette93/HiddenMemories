using UnityEngine;

public class CurrencyManager : MonoBehaviour, IDataPersistence
{
    public int Currency { get { return currency; } set { currency = value; } }
    int currency;

    private void Awake()
    {
        currency = 100;
        InteractableActions.AddCash += IncrementCurrency;
    }

    private void IncrementCurrency(ItemDataSO itemData)
    {
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
