using UnityEngine;

public class DMGtoaction : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Optional AudioSource used to play damage/death sound effects.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sound effect played when this entity takes damage.")]
    [SerializeField] private AudioClip damageClip;
    [Tooltip("Sound effect played when this entity dies.")]
    [SerializeField] private AudioClip deathClip;

    [Header("Spawn Prefabs")]
    [Tooltip("Prefab spawned when this entity takes damage.")]
    [SerializeField] private GameObject damageSpawnPrefab;
    [Tooltip("Prefab spawned when this entity dies.")]
    [SerializeField] private GameObject deathSpawnPrefab;
    [Tooltip("Local offset applied to spawned prefabs.")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private HealthSystem healthSystem;
    private Enemy enemyHealth;
    private float lastHealthPercentage = 100f;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        enemyHealth = GetComponent<Enemy>();
        if (enemyHealth != null)
        {
            lastHealthPercentage = enemyHealth.CurrentHealthPercentage;
            enemyHealth.OnHealthChanged.AddListener(HandleHealthChanged);
            enemyHealth.OnDeath.AddListener(HandleDeath);
            return;
        }

        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogWarning("DMGtoaction: HealthSystem or Enemy component not found on " + gameObject.name, gameObject);
            return;
        }

        lastHealthPercentage = healthSystem.GetHealthPercentage();
        healthSystem.OnHealthChanged.AddListener(HandleHealthChanged);
        healthSystem.OnDeath.AddListener(HandleDeath);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.RemoveListener(HandleHealthChanged);
            enemyHealth.OnDeath.RemoveListener(HandleDeath);
        }
        else if (healthSystem != null)
        {
            healthSystem.OnHealthChanged.RemoveListener(HandleHealthChanged);
            healthSystem.OnDeath.RemoveListener(HandleDeath);
        }
    }

    private void HandleHealthChanged(float newHealthPercentage)
    {
        if (newHealthPercentage < lastHealthPercentage)
        {
            PlayClip(damageClip);
            SpawnPrefab(damageSpawnPrefab);
        }

        lastHealthPercentage = newHealthPercentage;
    }

    private void HandleDeath()
    {
        PlayClip(deathClip);
        SpawnPrefab(deathSpawnPrefab);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        if (audioSource != null)
            audioSource.PlayOneShot(clip);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab == null)
            return;

        Instantiate(prefab, transform.position + spawnOffset, transform.rotation);
    }
}
