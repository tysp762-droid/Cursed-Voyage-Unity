using UnityEngine;

public class RayCastEnemy : RayCastSystem
{
    [SerializeField] private string playerTag = "Player";

    public override void HandleRaycast()
    {
        if (Raycast(transform.position, transform.forward))
        {
            if (hit.collider.CompareTag(playerTag))
                Debug.Log($"Enemy sees player at {hit.distance}m");
        }
    }
}
