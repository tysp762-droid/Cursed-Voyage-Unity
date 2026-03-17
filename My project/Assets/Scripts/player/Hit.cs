using UnityEngine;

public class Hit : RayCastSystem
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    public GameObject particlePrefab1;
    public GameObject particlePrefab2;

    public override void HandleRaycast()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            Debug.Log("Step 1: Right mouse button pressed");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Step 2: Ray created from camera to mouse position");
            if (Raycast(ray.origin, ray.direction))
            {
                Debug.Log("Step 3: Raycast hit something: " + hit.collider.gameObject.name);
                if (hit.collider.CompareTag(enemyTag))
                {
                    Debug.Log("Step 4: Hit object has Enemy tag");
                    // Deal damage to the enemy
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        Debug.Log("Step 5: Enemy component found, dealing " + damageAmount + " damage to " + hit.collider.gameObject.name);
                        enemy.DoDmg(damageAmount);
                        Debug.Log("Step 6: Damage dealt successfully");
                    }
                    else
                    {
                        Debug.Log("Error: No Enemy component on " + hit.collider.gameObject.name);
                    }

                    // Spawn particle systems at hit point
                    if (particlePrefab1 != null)
                    {
                        Instantiate(particlePrefab1, hit.point, Quaternion.identity);
                        Debug.Log("Step 7: Particle 1 spawned");
                    }
                    if (particlePrefab2 != null)
                    {
                        Instantiate(particlePrefab2, hit.point, Quaternion.identity);
                        Debug.Log("Step 8: Particle 2 spawned");
                    }
                }
                else
                {
                    Debug.Log("Error: Hit object does not have Enemy tag: " + hit.collider.tag);
                }
            }
            else
            {
                Debug.Log("Error: Raycast did not hit anything");
            }
        }
    }
}
