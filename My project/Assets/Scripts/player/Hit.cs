using UnityEngine;

public class Hit : RayCastSystem
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float shootCooldown = 0.5f; // Cooldown time in seconds, settable in Unity
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemsS0 requiredEquippedItem;
    [SerializeField] private GameObject requiredEquippedPrefab;
    [SerializeField] private string requiredEquippedItemName = "";
    [SerializeField] private string requiredEquippedPrefabName = "";
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackPressSound;
    [SerializeField] private AudioClip hitSound;
    public GameObject particlePrefab1;
    public GameObject particlePrefab2;

    private float lastShootTime;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();

        if (playerInventory == null)
            Debug.LogWarning("Hit: No Inventory found in scene. Assign playerInventory or ensure an Inventory component exists.");

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private bool IsMatchingPrefab(GameObject prefab, GameObject requiredPrefab)
    {
        if (prefab == null || requiredPrefab == null)
            return false;

        if (prefab == requiredPrefab)
            return true;

        return string.Equals(prefab.name, requiredPrefab.name, System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsMatchingPrefabName(GameObject prefab, string requiredName)
    {
        return prefab != null && string.Equals(prefab.name, requiredName, System.StringComparison.OrdinalIgnoreCase);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            return;
        }

        AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : transform.position);
    }

    private bool CanAttack()
    {
        if (playerInventory == null)
            return false;

        ItemsS0 equippedItem = playerInventory.GetEquippedItem();
        if (equippedItem == null)
            return false;

        if (requiredEquippedItem != null)
            return equippedItem == requiredEquippedItem;

        if (requiredEquippedPrefab != null)
        {
            if (IsMatchingPrefab(equippedItem.itemPrefab, requiredEquippedPrefab) || IsMatchingPrefab(equippedItem.handItemPrefab, requiredEquippedPrefab))
                return true;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredEquippedPrefabName))
        {
            if (IsMatchingPrefabName(equippedItem.itemPrefab, requiredEquippedPrefabName) || IsMatchingPrefabName(equippedItem.handItemPrefab, requiredEquippedPrefabName))
                return true;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredEquippedItemName))
            return string.Equals(equippedItem.itemName, requiredEquippedItemName, System.StringComparison.OrdinalIgnoreCase);

        return true;
    }

    public override void HandleRaycast()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= lastShootTime + shootCooldown) // Right mouse button with cooldown check
        {
            if (!CanAttack())
            {
                Debug.Log($"Attack blocked: must equip {requiredEquippedItemName} to attack.");
                return;
            }

            PlaySound(attackPressSound);
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
                    PlaySound(hitSound);
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
            if (!CanAttack())
            {
                Debug.Log($"Attack unavailable: equip {requiredEquippedItemName} first.");
            }
            else
            {
                Debug.Log("Cooldown active, cannot shoot yet");
            }
        }
    }
}
