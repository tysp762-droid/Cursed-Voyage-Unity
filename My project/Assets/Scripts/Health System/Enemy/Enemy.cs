using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f; // Set enemy HP in Unity Inspector
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void DoDmg(float damage)
    {
        Debug.Log("Enemy DoDmg called with damage: " + damage);
        Debug.Log("Enemy current health before damage: " + currentHealth + "/" + maxHealth);
        currentHealth -= damage;
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
        Destroy(gameObject);
    }
}