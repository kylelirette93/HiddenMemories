using UnityEngine;

public class CoinDataSO : ItemDataSO
{
    public int value;
    public override void Use()
    {
        CurrencyManager.Instance.IncrementCurrency(this);
    }
}
