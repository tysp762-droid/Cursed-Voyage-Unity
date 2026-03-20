using UnityEngine;
using UnityEngine.AI;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameObject enemy; // The enemy prefab to spawn
    [SerializeField] private float timeBetweenWaves = 5f; // Time between waves in seconds
    [SerializeField] private int enemiesPerWave = 3; // Number of enemies to spawn per wave
    [SerializeField] private Transform spawnPoint; // Where to spawn enemies
    [SerializeField] private float spawnSearchRadius = 10f; // Radius to search for valid NavMesh position

    private float nextWaveTime;

    void Start()
    {
        Debug.Log("WaveController Start - spawnPoint: " + spawnPoint + ", enemyPrefab: " + enemyPrefab);
        nextWaveTime = Time.time + timeBetweenWaves;
    }

    void Update()
    {
        if (Time.time >= nextWaveTime)
        {
            SpawnWave();
            nextWaveTime = Time.time + timeBetweenWaves;
        }
    }

    private void SpawnWave()
    {
        Debug.Log("SpawnWave called!");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            if (spawnPoint != null && enemy != null)
            {
                // Find a valid position on the NavMesh near the spawn point
                Vector3 spawnPosition = spawnPoint.position;
                if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, spawnSearchRadius, NavMesh.AllAreas))
                {
                    GameObject enemy = Instantiate(enemy, hit.position, spawnPoint.rotation);
                    Debug.Log("Enemy spawned at " + hit.position);
                    Debug.Log("Enemy name: " + enemy.name + ", Active: " + enemy.activeInHierarchy);
                    
                    // Check if it has a renderer
                    Renderer renderer = enemy.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Debug.Log("Renderer found, enabled: " + renderer.enabled);
                    }
                    else
                    {
                        Debug.LogWarning("No Renderer component found on spawned enemy!");
                    }
                    
                    // Check if it has ChasePlayer script
                    ChasePlayer chaseScript = enemy.GetComponent<ChasePlayer>();
                    if (chaseScript != null)
                    {
                        Debug.Log("ChasePlayer script found on enemy");
                    }
                    else
                    {
                        Debug.LogWarning("No ChasePlayer script found on spawned enemy!");
                    }
                    
                    // Wait a frame for NavMesh to initialize
                    StartCoroutine(DelayNavMeshSetup(enemy));
                }
                else
                {
                    Debug.LogError("NavMesh.SamplePosition failed! No valid NavMesh position found near " + spawnPosition);
                }
            }
            else
            {
                Debug.LogError("spawnPoint or enemyPrefab is not assigned!");
            }
        }
    }

    private System.Collections.IEnumerator DelayNavMeshSetup(GameObject Enemy)
    {
        yield return null; // Wait one frame
        if (enemy != null)
        {
            Debug.Log("After delay - Enemy still exists, active: " + enemy.activeInHierarchy);
            Renderer renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
            {
                Debug.Log("Renderer still enabled: " + renderer.enabled);
            }
        }
        else
        {
            Debug.LogError("Enemy was destroyed or is null after delay!");
        }
    }
}
