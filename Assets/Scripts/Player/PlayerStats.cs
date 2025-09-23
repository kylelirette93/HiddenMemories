using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    // Player's health.
    public int PlayerHealth { get { return playerHealth; } set { playerHealth = value; } }
    int playerHealth = 100;
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
        // Singleton pattern for accessing stats globally.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(SoulSubtraction());
    }

    IEnumerator SoulSubtraction()
    {
        while (soulHealth > 0)
        {
            yield return new WaitForSeconds(2.5f);
            soulHealth--;
        }
    }
}
