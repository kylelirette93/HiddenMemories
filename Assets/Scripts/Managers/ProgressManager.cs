using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public int EnemiesKilled { get { return enemiesKilled; } }
    int enemiesKilled;

    public int Currency { get { return currency; } }
    int currency;

    public void EnemyKilled()
    {
        enemiesKilled++;
    }

    public void CurrencyAdded()
    {
        currency++;
    }
    public void Reset()
    {
        enemiesKilled = 0;
        currency = 0;
    }
}
