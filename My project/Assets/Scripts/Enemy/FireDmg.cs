using UnityEngine;

public class FireDmg : MonoBehaviour
{
    public GameObject fireParticlePrefab;  // Sleep hier het particle system prefab in via inspector
    public float damagePerSecond = 10f;    // Hoeveel schade per seconde

    private Enemy enemyHealth;
    private bool isInFire = false;

    void Start()
    {
        enemyHealth = GetComponent<Enemy>();
        if (enemyHealth == null)
        {
            Debug.LogError("FireDmg: Geen Enemy component gevonden op dit object!");
        }
        if (fireParticlePrefab == null)
        {
            Debug.LogWarning("FireDmg: fireParticlePrefab is niet ingesteld in inspector.");
        }
    }

    void Update()
    {
        if (isInFire && enemyHealth != null)
        {
            enemyHealth.DoDmg(damagePerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fireParticlePrefab != null && IsInstanceOfPrefab(other.gameObject, fireParticlePrefab))
        {
            isInFire = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (fireParticlePrefab != null && IsInstanceOfPrefab(other.gameObject, fireParticlePrefab))
        {
            isInFire = false;
        }
    }

    // Helper functie om te checken of een object een instantie is van een prefab
    private bool IsInstanceOfPrefab(GameObject instance, GameObject prefab)
    {
        // Vergelijk de naam van het prefab met de naam van het object zonder (Clone)
        string instanceName = instance.name.Replace("(Clone)", "").Trim();
        return instanceName == prefab.name;
    }
}
