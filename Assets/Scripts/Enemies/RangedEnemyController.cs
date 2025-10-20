using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

public class RangedEnemyController : EnemyController
{
    public ParticleSystem vomitParticles;

    public override void AttackPlayer()
    {
        // Gonna fire a projectile here.
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            vomitParticles.Play();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(5);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
