using UnityEngine;

public abstract class RayCastSystem : MonoBehaviour
{
    [SerializeField] protected float raycastDistance = 10f;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected bool drawDebugRay = true;

    protected RaycastHit hit;

    protected bool Raycast(Vector3 origin, Vector3 direction)
    {
        if (drawDebugRay)
            Debug.DrawRay(origin, direction.normalized * raycastDistance, Color.green);

        return Physics.Raycast(origin, direction.normalized, out hit, raycastDistance, targetLayer);
    }

    public abstract void HandleRaycast();

    protected virtual void Update()
    {
        HandleRaycast();
    }
}
