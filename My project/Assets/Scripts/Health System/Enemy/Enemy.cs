using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f; // Set enemy HP in Unity Inspector
    private float currentHealth;

    public UnityEvent<float> OnHealthChanged = new UnityEvent<float>();
    public UnityEvent OnDeath = new UnityEvent();

    public float CurrentHealthPercentage => maxHealth <= 0f ? 0f : currentHealth / maxHealth * 100f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void DoDmg(float damage)
    {
        Debug.Log("Enemy DoDmg called with damage: " + damage);
        Debug.Log("Enemy current health before damage: " + currentHealth + "/" + maxHealth);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealthPercentage);
        Debug.Log("Enemy health after damage: " + currentHealth + "/" + maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy health <= 0, calling Die()");
            Die();
        }
        else
        {
            Debug.Log("Enemy still alive");
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}