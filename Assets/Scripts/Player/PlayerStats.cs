using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    // Amount of souls player has(decreases every second).
    public int SoulHealth
    {
        get => soulHealth;
    }
    int soulHealth = 100;

    public int MaxSoulHealth { get { return maxSoulHealth; } }
    int maxSoulHealth = 100;

    public int Currency { get { return currency; } set { currency = value; } }
    int currency = 0;

    public event Action<int> OnSoulHealthChanged;

    private void Awake()
    {
        #region Singleton Instance
        // Singleton pattern for accessing stats globally.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
        InteractableActions.AddCash += IncrementCurrency;
    }

    private void Start()
    {
        StartCoroutine(SoulSubtraction());
    }

    IEnumerator SoulSubtraction()
    {
        while (soulHealth > 0)
        {
            yield return new WaitForSeconds(0.6f);
            soulHealth--;
        }
        PlayerHealthActions.PlayerDied?.Invoke();
    }

    private void IncrementCurrency(ItemDataSO itemData)
    {
        currency += itemData.value;
        Debug.Log("Currency: " + currency);
    }

    public void IncrementSoulHealth()
    {
        soulHealth += 10;
    }
}
