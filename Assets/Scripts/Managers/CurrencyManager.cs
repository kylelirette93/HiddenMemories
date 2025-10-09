using UnityEngine;

public class CurrencyManager : MonoBehaviour
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
}
