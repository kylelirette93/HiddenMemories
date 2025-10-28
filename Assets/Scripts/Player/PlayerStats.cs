using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]

public class PlayerStats : MonoBehaviour
{
    // Amount of souls player has(decreases every second).
    public static PlayerStats Instance;
    public int SoulHealth
    {
        get => soulHealth;
    }
    int soulHealth = 100;

    public int MaxSoulHealth { get { return maxSoulHealth; } }
    int maxSoulHealth = 100;

    public event Action<int> OnSoulHealthChanged;
    public event Action OnSoulGained;

    public int MaxHealth { get { return maxHealth; } }
    int maxHealth = 100;
    int baseHealth = 100;
    public List<UpgradeDataSO> availableUpgrades = new List<UpgradeDataSO>();

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }
    private void OnEnable()
    {
        ApplyUpgrades();
        soulHealth = maxSoulHealth;
        StartCoroutine(SoulSubtraction());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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

    public void IncrementSoulHealth()
    {
        soulHealth += 10;
        OnSoulGained?.Invoke();
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        Debug.Log("Max Health increased to: " + maxHealth);
    }
    public void ResetToBaseStat()
    {
        maxHealth = baseHealth;
    }

    public void ApplyUpgrades()
    {
        ResetToBaseStat();
        foreach (var upgrade in availableUpgrades)
        {
            int tier = GameManager.Instance.upgradeManager.GetUpgradeTier(upgrade);
            if (tier > 0)
            {
                upgrade.Upgrade(null, tier);
            }
        }
    }
}
