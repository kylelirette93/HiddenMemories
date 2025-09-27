using System;
using UnityEngine;

public class EnemyHealth : Health
{
    public event Action OnEnemyDied;
    public override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0)
        {

            OnEnemyDied?.Invoke();
        }
    }
}
