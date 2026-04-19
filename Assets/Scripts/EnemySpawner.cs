using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnDistance = 12f;
    [SerializeField] private float groundY = -3f;

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
        Vector3 pos = new Vector3(spawnDistance * side, groundY, 0f);
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}