using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int CurrentHealth { get { return currentHealth; } }  
    protected int currentHealth = 100;
    public int MaxHealth { get { return maxHealth; } set { MaxHealth = value; } }
    protected int maxHealth = 100;

    public virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
    }
}
