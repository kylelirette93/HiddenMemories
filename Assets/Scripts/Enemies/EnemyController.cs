using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    protected NavMeshAgent agent;

    [SerializeField] protected Transform player;

    // Patroling.
    protected Vector3 walkPoint;
    protected bool walkPointSet;
    protected float walkPointRange;

    [SerializeField] protected LayerMask whatIsGround, whatIsPlayer;

    // Attacking
    [SerializeField] protected float timeBetweenAttacks;
    protected float moveSpeed;
    protected bool alreadyAttacked;
    protected int attackDamage;

    // Attack State variables
    [SerializeField] protected float sightRange, attackRange;
    protected bool playerInSightRange, playerInAttackRange;
    bool isAttacking = false;

    protected EnemyHealth health;
    [SerializeField] protected GameObject cashPrefab;
    protected bool canTakeDamage = true;
    protected bool isDead = false;
    [SerializeField] protected ParticleSystem bloodParticles;
    Animator animator;

    protected void Awake()
    {
        health = GetComponent<EnemyHealth>();
        StateActions.PlayerSpawned += SetPlayer;
        agent = GetComponent<NavMeshAgent>();
        health.OnEnemyDied += OnDeath;
        animator = GetComponentInChildren<Animator>();
    }
    protected void Update()
    {
        // Check if player is in sight or attack range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }
    protected void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        animator.SetBool("IsAttacking", isAttacking);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    protected void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    protected void ChasePlayer()
    {
        if (player != null)
        {
            animator.SetBool("IsAttacking", isAttacking);
            agent.SetDestination(player.position);
        }
    }

    public virtual void AttackPlayer()
    {
        agent.SetDestination(transform.position);


        RotateInstantlyTowardsTarget(transform, player.transform);

        if (!alreadyAttacked)
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", isAttacking);
            Debug.Log("Player being attacked by: " + gameObject.name);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            
        }
    }

    void RotateInstantlyTowardsTarget(Transform objectTransform, Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - objectTransform.position;
        direction.y = 0; // Ignore vertical difference
        objectTransform.rotation = Quaternion.LookRotation(direction);
    }

    protected void ResetAttack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(attackDamage);
        alreadyAttacked = false;
        isAttacking = false;
    }

    public virtual void TakeDamage(int damage, Vector3 contactPoint)
    {
        health.TakeDamage(damage);
        ParticleSystem particles = Instantiate(bloodParticles, contactPoint, transform.rotation);
    }

    protected void OnDeath()
    {
        if (isDead) return;
        isDead = true;
        GameManager.Instance.progressManager.EnemyKilled();
        health.OnEnemyDied -= OnDeath;
        PlayerStats.Instance.IncrementSoulHealth();
        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z);
        GameObject temp = Instantiate(cashPrefab, spawnPos, cashPrefab.transform.rotation);
        Debug.Log("Enemy's current position: " + transform.position);
        Debug.Log("Game object spawned at: " + spawnPos);
        GameManager.Instance.spawnManager.RemoveEnemy(gameObject);
    }


    protected void SetPlayer(GameObject player)
    {
        this.player = player.transform;
    }

    public void Initialize(EnemyData data)
    {
        moveSpeed = data.navSpeed;
        agent.speed = moveSpeed;
        timeBetweenAttacks = data.timeBetweenAttacks;
        attackDamage = data.attackDamage;
        Debug.Log("Enemy initialized with navSpeed: " + moveSpeed + ", timeBetweenAttacks: " + timeBetweenAttacks + ", attackDamage: " + attackDamage);
    }
}
