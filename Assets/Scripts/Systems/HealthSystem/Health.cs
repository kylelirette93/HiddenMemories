using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    int currentHealth = 100;
    int maxHealth = 100;
    public Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
