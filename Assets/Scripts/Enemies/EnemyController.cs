using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.InputSystem.Processors;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;

    Transform player;

    // Patroling.
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public LayerMask whatIsGround, whatIsPlayer;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Attack State variables
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private Health health;
    Material material;
    Color originalColor;
    public GameObject cashPrefab;
    bool canTakeDamage = true;
    bool isDead = false;


    private void Awake()
    {
        health = GetComponent<Health>();
        StateActions.PlayerSpawned += SetPlayer;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponentInChildren<Renderer>().material;
        health.OnDeath += OnDeath;
    }

    private void Update()
    {
        // Check if player is in sight or attack range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (!playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && canTakeDamage)
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        health.TakeDamage(20);
        canTakeDamage = false;
        originalColor = material.color;
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        material.color = originalColor;
        canTakeDamage = true;
    }

    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        health.OnDeath -= OnDeath;
        Instantiate(cashPrefab, transform.position + Vector3.up * 0.2f, cashPrefab.transform.rotation);
        Invoke("DestroySelf", 0.05f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        StateActions.PlayerSpawned -= SetPlayer;
    }

    private void SetPlayer(GameObject player)
    {
        this.player = player.transform;
    }
}
