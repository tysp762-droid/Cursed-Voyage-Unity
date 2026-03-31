using UnityEngine;

public class RayCastEnemy : RayCastSystem
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Damage")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Raycast")]
    [Tooltip("Offset in LOCAL space (relative to this enemy). Example: (0,1,0) = 1m omhoog vanaf pivot.")]
    [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, 1f, 0f);

    [Tooltip("Als aan: gebruikt transform.forward als richting (meestal goed voor 'aanvallen vooruit').")]
    [SerializeField] private bool useForwardDirection = true;

    [Tooltip("Extra rotatie (in graden) bovenop de richting, handig voor fine-tuning.")]
    [SerializeField] private Vector3 directionEulerOffset = Vector3.zero;

    private float timeSinceLastDamage;

    void Update()
    {
        timeSinceLastDamage += Time.deltaTime;
        base.Update();
    }

    public override void HandleRaycast()
    {
        // Origin met offset in lokale ruimte -> wereldruimte
        Vector3 origin = transform.TransformPoint(rayOriginOffset);

        // Richting (standaard forward), met optionele euler offset
        Vector3 dir = useForwardDirection ? transform.forward : transform.TransformDirection(Vector3.forward);
        dir = Quaternion.Euler(directionEulerOffset) * dir;

        // Gebruik onze eigen origin/direction i.p.v. transform.position/transform.forward
        if (Raycast(origin, dir))
        {
            if (hit.collider != null && hit.collider.CompareTag(playerTag))
            {
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

#if UNITY_EDITOR
    // Kleine debug-visualisatie in de editor zodat je meteen ziet waar de ray start.
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.TransformPoint(rayOriginOffset);
        Vector3 dir = (useForwardDirection ? transform.forward : transform.TransformDirection(Vector3.forward));
        dir = Quaternion.Euler(directionEulerOffset) * dir;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin, 0.04f);
        Gizmos.DrawRay(origin, dir * 2f);
    }
#endif
}
