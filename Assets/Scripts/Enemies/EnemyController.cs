using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    float movementSpeed = 5f;
    float attackRadius = 0.5f;
    bool isAttacking = false;
    EnemyState previousState;
    EnemyState currentState;
    GameObject player;
    Health health;
    Color originalColor;
    Color damageColor = Color.red;
    Material material;
    public GameObject cashPrefab;

    private void Start()
    {
        health = GetComponent<Health>();
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
        // Have enemy patrol by default.
        ChangeState(EnemyState.Patrol);
    }
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public void ChangeState(EnemyState state)
    {
        currentState = state;
    }

    public void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                // Patrol back and forth.
                break;
            case EnemyState.Chase:
                if (isAttacking)
                {
                    ChasePlayer();
                }
                // Chase the player.
                break;
            case EnemyState.Attack:
                isAttacking = true;
                // Attack the player if within radius.
                break;
        }
    }

    public void ChasePlayer()
    {
        // Get direction towards the player.
        Vector3 followDirection = (player.transform.position - transform.position).normalized;
        transform.position += followDirection * movementSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    IEnumerator FlashRed()
    {
        material.color = damageColor;
        yield return new WaitForSeconds(0.3f);
        material.color = originalColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            ChangeState(EnemyState.Chase);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(FlashRed());
            health.TakeDamage(20);
        }
    }

    private void OnDestroy()
    {
        Instantiate(cashPrefab, transform.position, Quaternion.identity);
    }
}
