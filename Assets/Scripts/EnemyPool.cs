using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public EnemyAI enemyPrefab;
    public int initialSize = 8;
    public int maxSize = 30;

    private ObjectPool<EnemyAI> pool;

    void Awake()
    {
        pool = new ObjectPool<EnemyAI>(
            CreateEnemy,
            OnGetEnemy,
            OnReleaseEnemy,
            OnDestroyEnemy,
            true,
            initialSize,
            maxSize
        );

        List<EnemyAI> preparedEnemies = new List<EnemyAI>();

        for (int i = 0; i < initialSize; i++)
        {
            preparedEnemies.Add(pool.Get());
        }

        foreach (EnemyAI enemy in preparedEnemies)
        {
            pool.Release(enemy);
        }
    }

    EnemyAI CreateEnemy()
    {
        EnemyAI enemy = Instantiate(enemyPrefab, transform);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.enabled = false;

        enemy.SetPool(this);
        enemy.gameObject.SetActive(false);

        return enemy;
    }

    void OnGetEnemy(EnemyAI enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    void OnReleaseEnemy(EnemyAI enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.enabled = false;

        enemy.gameObject.SetActive(false);
    }

    void OnDestroyEnemy(EnemyAI enemy)
    {
        Destroy(enemy.gameObject);
    }


    public EnemyAI Spawn(Vector3 position)
    {
        if (!NavMesh.SamplePosition(
            position,
            out NavMeshHit hit,
            5f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("Точка спавна вне NavMesh");
            return null;
        }

        EnemyAI enemy = pool.Get();

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        enemy.transform.position = hit.position;
        enemy.transform.rotation = Quaternion.identity;

        if (agent != null)
            agent.enabled = true;

        enemy.ResetEnemy();

        return enemy;
    }


    public void ReturnEnemy(EnemyAI enemy)
    {
        pool.Release(enemy);
    }

    public int ActiveCount => pool.CountActive;
    public int InactiveCount => pool.CountInactive;
}
