using UnityEngine;

public class VomitCollision : MonoBehaviour
{
    int vomitCooldown = 0;
    bool canDealDamage = true;
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (canDealDamage) 
                playerHealth.TakeDamage(5);
                canDealDamage = false;

            }
        }
    }

    private void Update()
    {
        if (!canDealDamage)
        {
            vomitCooldown++;
            if (vomitCooldown >= 90) 
            {
                canDealDamage = true;
                vomitCooldown = 0;
            }
        }
    }
}

