using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnDistance = 12f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        float side = Random.value > 0.5f ? 1f : -1f;
        Vector3 pos = transform.position + new Vector3(spawnDistance * side, 0f, 0f);
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}