using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    Rigidbody rb;
    MeshRenderer mr;
    TrailRenderer trailRenderer;
    public GameObject impactParticles;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
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
            Destroy(gameObject);
        }
        mr.enabled = false;
        GameObject temp = Instantiate(impactParticles, transform.position, impactParticles.transform.rotation);
        Destroy(gameObject);
    }
}
