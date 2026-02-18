using UnityEngine;

public class RayCastPlayer : RayCastSystem
{
    [SerializeField] private string enemyTag = "Enemy";

    public override void HandleRaycast()
    {
        if (Raycast(transform.position, transform.forward))
        {
            if (hit.collider.CompareTag(enemyTag))
                Debug.Log($"Player sees enemy at {hit.distance}m");
            else
                Debug.Log($"Player sees {hit.collider.name}");
        }
    }
}
