using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    public int maxWaves = 3;
    public int currentWave = 0;
    public int enemiesAlive = 0;

    public float spawnRadius = 10f;
    public float delayBetweenWaves = 3f;

    private PlayerHealth playerHealth;
    private bool waveStarting = false;
    private bool gameWon = false;

    public bool GameWon => gameWon;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        currentWave = 0;
        gameWon = false;

        if (enemyPrefab == null)
        {
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

        enemiesAlive =
            GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemiesAlive == 0)
        {
            if (currentWave >= maxWaves)
            {
                gameWon = true;
            }
            else
            {
                StartCoroutine(StartNextWave());
            }
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
            float angle = i * Mathf.PI * 2f / count;

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0.75f,
                Mathf.Sin(angle) * spawnRadius
            );

            Instantiate(enemyPrefab, position, Quaternion.identity);
        }

        enemiesAlive = count;

        waveStarting = false;
    }
}
