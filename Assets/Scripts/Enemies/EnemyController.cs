using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

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

    private EnemyHealth health;
    Material material;
    Color originalColor;
    public GameObject cashPrefab;
    bool canTakeDamage = true;
    bool isDead = false;
    public ParticleSystem bloodParticles;


    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        StateActions.PlayerSpawned += SetPlayer;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponentInChildren<Renderer>().material;
        health.OnEnemyDied += OnDeath;
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

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
        ParticleSystem particles = Instantiate(bloodParticles, transform.position + new Vector3(0, 0, 0.2f), Quaternion.identity);
    }

    public void OnDeath()
    {
        if (isDead) return;
        isDead = true;
        health.OnEnemyDied -= OnDeath;
        PlayerStats.Instance.IncrementSoulHealth();
        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z);
        GameObject temp = Instantiate(cashPrefab, spawnPos, cashPrefab.transform.rotation);
        Debug.Log("Enemy's current position: " + transform.position);
        Debug.Log("Game object spawned at: " + spawnPos);
        GameManager.Instance.spawnManager.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    private void SetPlayer(GameObject player)
    {
        this.player = player.transform;
    }
}
