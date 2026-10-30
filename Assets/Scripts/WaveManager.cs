using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public EnemyPool enemyPool;

    public int maxWaves = 3;
    public int currentWave = 0;
    public int enemiesAlive = 0;

    [Header("Enemy Spawn")]
    public Camera mainCamera;
    public float minSpawnDistance = 18f;
    public float maxSpawnDistance = 28f;
    public float spawnHeight = 1f;
    public int spawnAttempts = 40;
    public float delayBetweenWaves = 3f;

    private PlayerHealth playerHealth;
    private bool waveStarting = false;
    private bool gameWon = false;
   
    public bool GameWon => gameWon;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (enemyPool == null)
            enemyPool = FindFirstObjectByType<EnemyPool>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        currentWave = 0;
        gameWon = false;

        if (enemyPool == null)
        {
            Debug.LogError("WaveManager: EnemyPool не найден!");
            enabled = false;
            return;
        }

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (gameWon || waveStarting)
            return;

        if (playerHealth == null || playerHealth.IsDead())
            return;

        enemiesAlive = enemyPool.ActiveCount;

        if (enemiesAlive == 0)
        {
            if (currentWave >= maxWaves)
                gameWon = true;
            else
                StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        waveStarting = true;

        yield return new WaitForSeconds(delayBetweenWaves);

        if (playerHealth == null || playerHealth.IsDead())
        {
            waveStarting = false;
            yield break;
        }

        currentWave++;

        int count = 2 + currentWave * 2;

        for (int i = 0; i < count; i++)
        {
            Vector3 position = FindSpawnPosition();
            enemyPool.Spawn(position);
        }

        enemiesAlive = count;
        waveStarting = false;
    }

    Vector3 FindSpawnPosition()
    {
        Vector3 playerPosition = playerHealth.transform.position;

        for (int attempt = 0; attempt < spawnAttempts; attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(
                minSpawnDistance,
                maxSpawnDistance
            );

            Vector3 position = playerPosition + new Vector3(
                Mathf.Cos(angle) * distance,
                0f,
                Mathf.Sin(angle) * distance
            );

            position.y = spawnHeight;

            Vector3 viewportPoint =
                mainCamera.WorldToViewportPoint(position);

            bool visible =
                viewportPoint.z > 0f &&
                viewportPoint.x > -0.1f &&
                viewportPoint.x < 1.1f &&
                viewportPoint.y > -0.1f &&
                viewportPoint.y < 1.1f;

            if (!visible)
                return position;
        }

        // запасной вариант
        return playerPosition
            - mainCamera.transform.forward * maxSpawnDistance
            + Vector3.up * (spawnHeight - playerPosition.y);
    }
}
