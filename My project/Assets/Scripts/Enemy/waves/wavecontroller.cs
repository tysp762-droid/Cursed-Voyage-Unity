using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

public class WaveController : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Sleep hier meerdere enemy prefabs in.")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave settings")]
    [Min(0f)]
    public float timeBetweenWaves = 5f; // Nu handmatig instelbaar

    [Min(1)]
    public int enemiesPerWave = 10; // Nu handmatig instelbaar

    [Header("Spawn locations")]
    [Tooltip("Sleep hier meerdere spawnpunten in.")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("NavMesh (optioneel)")]
    [SerializeField] private bool useNavMesh = true;

    [Min(0f)]
    [SerializeField] private float navMeshSearchRadius = 10f;

    [Header("Debug")]
    [SerializeField] private bool logSpawns = false;

    [Header("Difficulty Settings - Time Between Waves")]
    public float easyTimeBetweenWaves = 20f;
    public float normalTimeBetweenWaves = 13f;
    public float hardTimeBetweenWaves = 3f;

    private DifficultyLevel currentDifficulty = DifficultyLevel.Easy;

    private Coroutine waveRoutine;

    private void Awake()
    {
        // Stel standaard moeilijkheid in op Easy, alleen voor timeBetweenWaves
        SetDifficulty(DifficultyLevel.Easy);
    }

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

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"{nameof(WaveController)}: Geen spawnpunten toegewezen. Gebruik transform van dit object als spawnpunt.");
            spawnPoints = new Transform[] { transform };
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
        int spawnPointCount = spawnPoints.Length;
        int baseEnemiesPerSpawnPoint = enemiesPerWave / spawnPointCount;
        int remainder = enemiesPerWave % spawnPointCount;

        for (int i = 0; i < spawnPointCount; i++)
        {
            int enemiesToSpawnHere = baseEnemiesPerSpawnPoint;
            if (i < remainder)
            {
                enemiesToSpawnHere += 1; // Verdeel de rest over de eerste spawnpunten
            }

            for (int j = 0; j < enemiesToSpawnHere; j++)
            {
                Transform spawnPoint = spawnPoints[i];
                Vector3 basePos = spawnPoint.position;
                Quaternion rot = spawnPoint.rotation;

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

                GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                Instantiate(prefabToSpawn, spawnPos, rot);

                if (logSpawns)
                {
                    Debug.Log($"[WaveController] Spawned: {prefabToSpawn.name} @ {spawnPos}");
                }
            }
        }
    }

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;

        // Alleen tijd tussen waves aanpassen, niet aantal vijanden
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                timeBetweenWaves = easyTimeBetweenWaves;
                break;
            case DifficultyLevel.Normal:
                timeBetweenWaves = normalTimeBetweenWaves;
                break;
            case DifficultyLevel.Hard:
                timeBetweenWaves = hardTimeBetweenWaves;
                break;
        }

        Debug.Log($"Difficulty set to {difficulty}: TimeBetweenWaves={timeBetweenWaves}, EnemiesPerWave={enemiesPerWave}");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                    Gizmos.DrawWireSphere(spawnPoint.position, useNavMesh ? navMeshSearchRadius : 0.25f);
            }
        }
        else
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, useNavMesh ? navMeshSearchRadius : 0.25f);
        }
    }
#endif
}
