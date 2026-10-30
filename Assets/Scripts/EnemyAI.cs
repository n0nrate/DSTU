using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float speed = 2.5f;
    public float health = 50f;
    public float damage = 5f;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Health")]
    public EnemyHealthBar healthBar;

    [Header("Hit Feedback")]
    public Renderer enemyRenderer;
    public float flashDuration = 0.12f;

    [Header("Energy Drop")]
    public GameObject energyCorePrefab;

    [Header("Navigation")]
    public float pathUpdateInterval = 0.2f;
    public LayerMask obstacleMask;

    [Header("Drone Rotation")]
    public Transform droneVisual;
    public float rotationSpeed = 10f;
    public float modelYawOffset = 0f;

    private float maxHealth;
    private float nextAttackTime;
    private float nextPathUpdateTime;

    private Transform player;
    private PlayerHealth playerHealth;
    private EnemyPool enemyPool;
    private NavMeshAgent agent;

    private Material enemyMaterial;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDead;

    void Awake()
    {
        maxHealth = health;

        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = speed;
            agent.stoppingDistance = attackDistance;
            agent.updateRotation = false;
        }

        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
        {
            enemyMaterial = enemyRenderer.material;

            if (enemyMaterial.HasProperty("_BaseColor"))
                originalColor = enemyMaterial.GetColor("_BaseColor");
        }
    }

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            player = playerHealth.transform;
    }

    void Update()
    {
        if (isDead || player == null || playerHealth == null)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        if (playerHealth.IsDead())
        {
            agent.isStopped = true;
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        // Проверяем препятствие между врагом и игроком.
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 target = player.position + Vector3.up * 0.5f;

        bool blocked = Physics.Linecast(
            origin,
            target,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        if (distance > attackDistance || blocked)
        {
            agent.isStopped = false;

            if (Time.time >= nextPathUpdateTime)
            {
                agent.SetDestination(player.position);
                nextPathUpdateTime =
                    Time.time + pathUpdateInterval;
            }
        }
        else
        {
            agent.isStopped = true;

            if (Time.time >= nextAttackTime)
            {
                playerHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void LateUpdate()
    {
        if (isDead || player == null || droneVisual == null)
            return;

        Vector3 direction = player.position - droneVisual.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction) *
            Quaternion.Euler(0f, modelYawOffset, 0f);

        droneVisual.rotation = Quaternion.Slerp(
            droneVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetPool(EnemyPool pool)
    {
        enemyPool = pool;
    }

    public void ResetEnemy()
    {
        StopAllCoroutines();
        flashCoroutine = null;

        health = maxHealth;
        isDead = false;

        nextAttackTime = Time.time + attackCooldown;
        nextPathUpdateTime = 0f;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }

        if (enemyMaterial != null &&
            enemyMaterial.HasProperty("_BaseColor"))
        {
            enemyMaterial.SetColor("_BaseColor", originalColor);
        }

        if (healthBar != null)
            healthBar.UpdateHealth(health, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        if (GameAudio.Instance != null)
            GameAudio.Instance.PlayEnemyHit();

        health = Mathf.Max(0f, health - amount);

        if (healthBar != null)
            healthBar.UpdateHealth(health, maxHealth);

        if (health <= 0f)
        {
            Die();
            return;
        }

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashOnHit());
    }

    IEnumerator FlashOnHit()
    {
        if (enemyMaterial == null ||
            !enemyMaterial.HasProperty("_BaseColor"))
            yield break;

        enemyMaterial.SetColor("_BaseColor", Color.white);

        yield return new WaitForSeconds(flashDuration);

        enemyMaterial.SetColor("_BaseColor", originalColor);
        flashCoroutine = null;
    }

    void Die()
    {
        isDead = true;

        if (energyCorePrefab != null)
        {
            Vector3 dropPosition = transform.position;
            dropPosition.y = 0.6f;

            Instantiate(
                energyCorePrefab,
                dropPosition,
                Quaternion.identity
            );
        }

        if (GameAudio.Instance != null)
            GameAudio.Instance.PlayEnemyDeath();

        if (enemyPool != null)
            enemyPool.ReturnEnemy(this);
        else
            Destroy(gameObject);
    }
}
