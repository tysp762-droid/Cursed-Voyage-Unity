using UnityEngine;

public class RaycastNew : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask = ~0;

    void Update()
    {
        // Use left click (mouse0) once per press.
        if (Input.GetMouseButtonDown(0)) // left click (mouse0)
        {
            ShootRayFromObject();
        }
    }

    void ShootRayFromObject()
    {
        // Determine origin (the GameObject this script is attached to)
        Vector3 origin = transform.position;

        // Prefer camera forward if available, otherwise use the object's forward
        Vector3 direction = (cam != null) ? cam.transform.forward : transform.forward;

        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }
    }
}
