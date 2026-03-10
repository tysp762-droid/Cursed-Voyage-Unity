using UnityEngine;

public class RayCastEnemy : RayCastSystem
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1f;

    private float timeSinceLastDamage;

    void Update()
    {
        timeSinceLastDamage += Time.deltaTime;
        base.Update();
    }

    public override void HandleRaycast()
    {
        if (Raycast(transform.position, transform.forward))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                // Deal damage to the player
                if (timeSinceLastDamage >= damageCooldown)
                {
                    HealthSystem playerHealth = hit.collider.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damageAmount);
                        timeSinceLastDamage = 0f;
                        Debug.Log("Player took damage!");
                    }
                }
            }
        }
    }
}
