using UnityEngine;

public class RayCast : MonoBehaviour
{
    public Camera cam;                // assign main camera in inspector (or leave null to auto-find)
    public float maxDistance = 100f;
    public LayerMask hitLayers = ~0;  // default: everything

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
            {
                Debug.Log($"Hit {hit.collider.name} at {hit.point}");
                // Example: interact with the hit object
                // var hitObj = hit.collider.gameObject;
                // hitObj.GetComponent<Health>()?.TakeDamage(10);
            }
            else
            {
                Debug.Log("No hit");
            }

            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 1f);
        }
    }
}
