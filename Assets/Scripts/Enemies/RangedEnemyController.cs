using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

public class RangedEnemyController : EnemyController
{
    public override void AttackPlayer()
    {
        // Gonna fire a projectile here.
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(5);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
