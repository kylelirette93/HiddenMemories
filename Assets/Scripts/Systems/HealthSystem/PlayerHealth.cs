using System;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlayerHealth : Health
{
    PlayerStats playerStats;
    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    public void OnEnable()
    {
        maxHealth = playerStats.MaxHealth;
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(maxHealth, maxHealth);
    }
    public override void Start()
    {
        base.Start();
        maxHealth = playerStats.MaxHealth;
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(maxHealth, maxHealth);
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            PlayerHealthActions.PlayerDied?.Invoke();
            Heal(maxHealth);
        }
    }

    public void Heal(int amount)    
    {
        currentHealth += amount;
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
public static class PlayerHealthActions
{
    public static Action<int, int> OnPlayerHealthChanged;
    // Player died, notify game state manager.
    public static Action PlayerDied;
}
