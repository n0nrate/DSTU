using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2.5f;
    public float health = 50f;
    public float damage = 5f;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private PlayerHealth playerHealth;
    private float nextAttackTime;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            player = playerHealth.transform;
    }

    void Update()
    {
        if (player == null || playerHealth.IsDead())
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > attackDistance)
        {
            transform.position +=
                direction.normalized * speed * Time.deltaTime;
        }

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            playerHealth.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
            Destroy(gameObject);
    }
}
