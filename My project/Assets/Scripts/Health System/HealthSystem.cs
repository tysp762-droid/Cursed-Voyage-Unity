using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f; // Enemy HP amount
    private float currentHealth;

    // Events for other systems to respond to
    public UnityEvent<float> OnHealthChanged; // Passes health percentage (0-100)
    public UnityEvent OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        float percentage = GetHealthPercentage();
        Debug.Log("HealthSystem: Taking damage, new health: " + currentHealth + ", percentage: " + percentage);
        OnHealthChanged?.Invoke(percentage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        float percentage = GetHealthPercentage();
        Debug.Log("HealthSystem: Healing, new health: " + currentHealth + ", percentage: " + percentage);
        OnHealthChanged?.Invoke(percentage);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        // Removed Destroy(gameObject) to allow custom death handling
    }

    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth) * 100f;
    }

    // Nieuw: detecteer collision met death barrier prefab
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBarrier"))
        {
            Debug.Log("HealthSystem: Hit DeathBarrier, dying instantly.");
            Die();
        }
    }
}
