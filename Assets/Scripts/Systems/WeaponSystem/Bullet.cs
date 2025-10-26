using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    Rigidbody rb;
    ParticleSystem impactParticles;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        impactParticles = GetComponentInChildren<ParticleSystem>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            ContactPoint contact = collision.GetContact(0);
            Vector3 contactPoint = contact.point;
            if (enemy != null)
            {
                enemy.TakeDamage((int)Damage * 10, contactPoint);
            }
            Destroy(gameObject, 0.1f);
        }
        else
        {
            impactParticles.Play();
            Destroy(gameObject, 0.1f);
        }
    }
}
