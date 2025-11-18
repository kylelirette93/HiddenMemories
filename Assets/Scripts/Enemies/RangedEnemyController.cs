using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : EnemyController
{
    public ParticleSystem vomitParticles;

    public override void AttackPlayer()
    {
        base.AttackPlayer();
    }

    public override IEnumerator AttackSequence()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        GameManager.Instance.audioManager.PlaySound("vomit");
        vomitParticles.Play();
        float animationHitTime = timeBetweenAttacks * 0.5f;
        yield return new WaitForSeconds(animationHitTime);

        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        alreadyAttacked = false;
        agent.isStopped = false;
    }

    public void DealDamageToPlayer()
    {
        playerHealth.TakeDamage(5);
    }

    public override void TakeDamage(int damage, Vector3 contactPoint)
    {
        base.TakeDamage(damage, contactPoint);
    }
}
