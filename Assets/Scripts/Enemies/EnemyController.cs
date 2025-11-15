using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
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
    public AudioClip demon_grunt;
    bool isInAttackSequence = false;
    public AudioClip demon_die;
    public ParticleSystem explosionParticles;
    public ParticleSystem soulParticles;
    private CapsuleCollider collider;
    PlayerHealth playerHealth;

    protected void Awake()
    {
        health = GetComponent<EnemyHealth>();
        StateActions.PlayerSpawned += SetPlayer;
        agent = GetComponent<NavMeshAgent>();
        health.OnEnemyDied += OnDeath;
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
        }
    }

    private void OnDisable()
    {
        StateActions.PlayerSpawned -= SetPlayer;
        if (health != null)
        {
            health.OnEnemyDied -= OnDeath;
        }
    }
    protected void Update()
    {
        if (isDead) return;
        // Check if player is in sight or attack range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange - 0.5f, whatIsPlayer);

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
        {
            walkPointSet = false;
        }
        isInAttackSequence = false;
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
            isInAttackSequence = false;
        }
    }

    public virtual void AttackPlayer()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth.CurrentHealth <= 0) return; 
        agent.isStopped = true;
        RotateInstantlyTowardsTarget(transform, player.transform);
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            if (!isDead)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        // Play grunt sound at start of attack.
        GameManager.Instance.audioManager.PlaySound("demon_grunt");
        float animationHitTime = timeBetweenAttacks * 0.5f; 
        yield return new WaitForSeconds(animationHitTime);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (playerHealth != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.ShakeCam();
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }
        
        // Wait for rest of animation to complete.
        yield return new WaitForSeconds(timeBetweenAttacks - animationHitTime);

        // Reset attack state
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        alreadyAttacked = false;
        agent.isStopped = false;
    }

    void RotateInstantlyTowardsTarget(Transform objectTransform, Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - objectTransform.position;
        direction.y = 0; // Ignore vertical difference
        objectTransform.rotation = Quaternion.LookRotation(direction);
    }

    protected void ResetAttack()
    {
        alreadyAttacked = false;
        isAttacking = false;
        isInAttackSequence = false;
    }

    public virtual void TakeDamage(int damage, Vector3 contactPoint)
    {
        health.TakeDamage(damage);
        ParticleSystem particles = Instantiate(bloodParticles, contactPoint, transform.rotation);
    }

    protected void OnDeath()
    {
        Vector3 respawnPosition = transform.position;
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            // Reset nav mesh agent on death.
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
        collider.enabled = false;
        animator.SetBool("IsAttacking", false);
        animator.SetTrigger("Death");
        //GameObject explosion = Instantiate(explosionParticles.gameObject, transform.position, Quaternion.identity);
        SpawnSoul();
        GameManager.Instance.progressManager.EnemyKilled();
        health.OnEnemyDied -= OnDeath;
        Vector3 spawnPos = new Vector3(transform.position.x, 1.5f, transform.position.z);
        GameObject temp = Instantiate(cashPrefab, spawnPos, cashPrefab.transform.rotation);
        //Debug.Log("Enemy's current position: " + transform.position);
        //Debug.Log("Game object spawned at: " + spawnPos);
        GameManager.Instance.audioManager.PlaySound("demon_die");
        GameManager.Instance.spawnManager.RemoveEnemy(gameObject);
        GameManager.Instance.spawnManager.RespawnEnemyAtPosition(respawnPosition, 20f);
    }

    private void SpawnSoul()
    {
        Camera mainCam = Camera.main;
        GameObject soul = Instantiate(soulParticles.gameObject, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        RectTransform soulMeter = GameManager.Instance.hud.soulMeterSlider.GetComponent<RectTransform>();

        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = soul.transform.position;

        DOTween.To(() => elapsed, x => elapsed = x, duration, duration)
            .OnUpdate(() =>
            {
                if (soul != null)
                {
                    Vector3 targetPos = mainCam.ScreenToWorldPoint(new Vector3(soulMeter.position.x - 50f, soulMeter.position.y, 1));
                    soul.transform.position = Vector3.Lerp(startPos, targetPos, DOVirtual.EasedValue(0, 1, elapsed / duration, Ease.InQuad));
                }
            })
            .OnComplete(() =>
            {
                Destroy(soul.gameObject);
                PlayerStats.Instance.IncrementSoulHealth();
            })
            .SetLink(gameObject);
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
        //Debug.Log("Enemy initialized with navSpeed: " + moveSpeed + ", timeBetweenAttacks: " + timeBetweenAttacks + ", attackDamage: " + attackDamage);
    }
}
