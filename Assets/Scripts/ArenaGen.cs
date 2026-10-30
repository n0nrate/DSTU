
using System.Collections.Generic;
using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    public GameObject wallPrefab;

    [Header("Generation")]
    public int wallCount = 10;
    public int gridSize = 4;
    public float generationHalfSize = 24f;
    public float safeRadius = 8f;
    public float wallSpacing = 3f;
    public float wallY = 1.5f;

    void Start()
    {
        GenerateWalls();
    }

    void GenerateWalls()
    {

        List<Vector2Int> sectors = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                sectors.Add(new Vector2Int(x, z));
            }
        }

        for (int i = sectors.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Vector2Int temp = sectors[i];
            sectors[i] = sectors[randomIndex];
            sectors[randomIndex] = temp;
        }

        float cellSize = generationHalfSize * 2f / gridSize;
        int created = 0;

        foreach (Vector2Int sector in sectors)
        {
            if (created >= wallCount)
                break;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                float x = -generationHalfSize
                    + (sector.x + 0.5f) * cellSize
                    + Random.Range(-0.2f, 0.2f) * cellSize;

                float z = -generationHalfSize
                    + (sector.y + 0.5f) * cellSize
                    + Random.Range(-0.2f, 0.2f) * cellSize;

                Vector3 position = new Vector3(x, wallY, z);

                if (new Vector2(x, z).magnitude < safeRadius)
                    continue;

                Quaternion rotation = Quaternion.Euler(
                    0f, Random.Range(0f, 180f), 0f
                );

                Vector3 halfExtents =
                    Vector3.Scale(
                        wallPrefab.transform.localScale,
                        Vector3.one * 0.5f
                    );

                halfExtents.x += wallSpacing * 0.5f;
                halfExtents.z += wallSpacing * 0.5f;

                Collider[] overlaps = Physics.OverlapBox(
                    position,
                    halfExtents,
                    rotation
                );

                bool blocked = false;

                foreach (Collider col in overlaps)
                {
                    if (col.gameObject.name == "Floor")
                        continue;

                    blocked = true;
                    break;
                }

                if (blocked)
                    continue;

                Instantiate(
                    wallPrefab,
                    position,
                    rotation,
                    transform
                );

                created++;
                break;
            }
        }
    }
}
