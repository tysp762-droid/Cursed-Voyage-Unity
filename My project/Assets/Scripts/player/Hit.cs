using UnityEngine;

public class Hit : RayCastSystem
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float shootCooldown = 0.5f; // Cooldown time in seconds, settable in Unity
    public GameObject particlePrefab1;
    public GameObject particlePrefab2;

    private float lastShootTime;

    public override void HandleRaycast()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= lastShootTime + shootCooldown) // Right mouse button with cooldown check
        {
            lastShootTime = Time.time; // Update last shoot time
            Debug.Log("Step 1: Right mouse button pressed (cooldown passed)");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Step 2: Ray created from camera to mouse position");
            if (Raycast(ray.origin, ray.direction))
            {
                Debug.Log("Step 3: Raycast hit something: " + hit.collider.gameObject.name);

                // Try to deal damage if the object has an Enemy component
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log("Step 4: Enemy component found, dealing " + damageAmount + " damage to " + hit.collider.gameObject.name);
                    enemy.DoDmg(damageAmount);
                    Debug.Log("Step 5: Damage dealt successfully");
                }
                else
                {
                    Debug.Log("Step 4: Hit object has no Enemy component (no damage applied)");
                }

                // Spawn particle systems at hit point (always spawn on hit)
                if (particlePrefab1 != null)
                {
                    Instantiate(particlePrefab1, hit.point, Quaternion.identity);
                    Debug.Log("Step 6: Particle 1 spawned");
                }
                if (particlePrefab2 != null)
                {
                    Instantiate(particlePrefab2, hit.point, Quaternion.identity);
                    Debug.Log("Step 7: Particle 2 spawned");
                }
            }
            else
            {
                Debug.Log("Error: Raycast did not hit anything");
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Cooldown active, cannot shoot yet");
        }
    }
}
