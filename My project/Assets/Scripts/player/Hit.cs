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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Raycast(ray.origin, ray.direction))
            {
                if (hit.collider.CompareTag(enemyTag))
                {
                    // Deal damage to the enemy
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.DoDmg(damageAmount);
                    }

                    // Spawn particle systems at hit point
                    if (particlePrefab1 != null)
                    {
                        Instantiate(particlePrefab1, hit.point, Quaternion.identity);
                    }
                    if (particlePrefab2 != null)
                    {
                        Instantiate(particlePrefab2, hit.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}
