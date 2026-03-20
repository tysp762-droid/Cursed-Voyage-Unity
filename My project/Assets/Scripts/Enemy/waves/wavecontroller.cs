using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WaveController : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Sleep hier meerdere enemy prefabs in.")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave settings")]
    [Min(0f)]
    [SerializeField] private float timeBetweenWaves = 5f;

    [Min(1)]
    [SerializeField] private int enemiesPerWave = 3;

    [Header("Spawn location")]
    [Tooltip("Laat leeg om de positie van dit GameObject te gebruiken.")]
    [SerializeField] private Transform spawnPoint;

    [Header("NavMesh (optioneel)")]
    [SerializeField] private bool useNavMesh = true;

    [Min(0f)]
    [SerializeField] private float navMeshSearchRadius = 10f;

    [Header("Debug")]
    [SerializeField] private bool logSpawns = false;

    private Coroutine waveRoutine;

    private void OnEnable()
    {
        StartWaves();
    }

    private void OnDisable()
    {
        StopWaves();
    }

    public void StartWaves()
    {
        if (waveRoutine != null) return;
        waveRoutine = StartCoroutine(WaveLoop());
    }

    public void StopWaves()
    {
        if (waveRoutine == null) return;
        StopCoroutine(waveRoutine);
        waveRoutine = null;
    }

    private IEnumerator WaveLoop()
    {
        yield return null; // kleine delay voor setup

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError($"{nameof(WaveController)}: Geen enemy prefabs toegewezen. Sleep minstens één prefab in 'Prefabs'.", this);
            waveRoutine = null;
            yield break;
        }

        while (enabled && gameObject.activeInHierarchy)
        {
            SpawnWave();

            if (timeBetweenWaves <= 0f)
            {
                yield return null; // voorkom tight loop
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        waveRoutine = null;
    }

    private void SpawnWave()
    {
        Transform actualSpawnPoint = spawnPoint != null ? spawnPoint : transform;
        Vector3 basePos = actualSpawnPoint.position;
        Quaternion rot = actualSpawnPoint.rotation;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPos = basePos;

            if (useNavMesh)
            {
                if (NavMesh.SamplePosition(basePos, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
                {
                    spawnPos = hit.position;
                }
                else
                {
                    Debug.LogWarning($"{nameof(WaveController)}: Geen NavMesh positie gevonden binnen radius {navMeshSearchRadius}. Spawnt op base positie.", this);
                    spawnPos = basePos;
                }
            }

            // Kies willekeurig een prefab uit de array
            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject spawnedEnemy = Instantiate(prefabToSpawn, spawnPos, rot);

            if (logSpawns)
            {
                Debug.Log($"[WaveController] Spawned: {spawnedEnemy.name} @ {spawnPos}", spawnedEnemy);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Transform actualSpawnPoint = spawnPoint != null ? spawnPoint : transform;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        Gizmos.DrawWireSphere(actualSpawnPoint.position, useNavMesh ? navMeshSearchRadius : 0.25f);
    }
#endif
}

