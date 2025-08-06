using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public float      spawnTimer;
        public float      spawnInterval = 1.2f;
        public int        enemiesPerWave = 30;
        public int        spawnedEnemyCount;
    }

    [Header("Wave list")]
    public List<Wave> waves = new List<Wave>();
    public int waveNumber = 0;

    [Header("Spawn rectangle (two opposite corners)")]
    public Transform minPos;
    public Transform maxPos;

    [Header("Spawn-rate scaling (every wave)")]
    [SerializeField] private float spawnIntervalScale = 0.83f;
    [SerializeField] private float minSpawnInterval   = 0.12f;

    [Header("Stat scaling (every N waves)")]
    [SerializeField] private int   scaleEveryXWaves = 4;
    [SerializeField] private float healthScale  = 1.15f;
    [SerializeField] private float speedScale   = 1.05f;
    [SerializeField] private float damageScale  = 1.10f;
    private float healthMul = 1f;
    private float speedMul  = 1f;
    private float damageMul = 1f;

    private int   wavesCompleted = 0;

    void Update()
    {
        if (!PlayerController.Instance.gameObject.activeSelf) return;

        Wave w = waves[waveNumber];

        w.spawnTimer += Time.deltaTime;
        if (w.spawnTimer >= w.spawnInterval)
        {
            w.spawnTimer = 0f;
            SpawnEnemy(w.enemyPrefab);
            w.spawnedEnemyCount++;
        }

        if (w.spawnedEnemyCount >= w.enemiesPerWave)
        {
            w.spawnedEnemyCount = 0;
            w.spawnInterval = Mathf.Max(minSpawnInterval,
                                        w.spawnInterval * spawnIntervalScale);

            wavesCompleted++;

            if (wavesCompleted % scaleEveryXWaves == 0)
            {
                healthMul *= healthScale;
                speedMul  *= speedScale;
                damageMul *= damageScale;
            }

            waveNumber++;
            if (waveNumber >= waves.Count) waveNumber = 0;
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Vector3 pos = RandomSpawnPoint();
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);

        Enemy e = go.GetComponent<Enemy>();
        if (e != null)
            e.ScaleStats(healthMul, speedMul, damageMul);
    }

    private Vector3 RandomSpawnPoint()
    {
        Vector3 p;
        if (Random.value > 0.5f)
        {
            p.x = Random.Range(minPos.position.x, maxPos.position.x);
            p.y = Random.value > 0.5f ? minPos.position.y : maxPos.position.y;
        }
        else
        {
            p.y = Random.Range(minPos.position.y, maxPos.position.y);
            p.x = Random.value > 0.5f ? minPos.position.x : maxPos.position.x;
        }
        p.z = 0f;
        return p;
    }
}