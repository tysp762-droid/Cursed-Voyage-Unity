using UnityEngine;

public class Hit : RayCastSystem
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float shootCooldown = 0.5f; // Cooldown time in seconds, settable in Unity
    [SerializeField] private string cooldownIdentifier = "";
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemsS0 requiredEquippedItem;
    [SerializeField] private GameObject requiredEquippedPrefab;
    [SerializeField] private string requiredEquippedItemName = "";
    [SerializeField] private string requiredEquippedPrefabName = "";
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject particlePrefab1;
    [SerializeField] private GameObject particlePrefab2;
    [SerializeField] private float particleLifetime = 2f;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackPressSound;
    [SerializeField] private AudioClip hitSound;

    private string cooldownKey;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();

        if (playerInventory == null)
            Debug.LogWarning("Hit: No Inventory found in scene. Assign playerInventory or ensure an Inventory component exists.");

        cooldownKey = GetCooldownKey();

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

    private string GetCooldownKey()
    {
        if (!string.IsNullOrWhiteSpace(cooldownIdentifier))
            return cooldownIdentifier;

        if (playerInventory != null)
        {
            ItemsS0 equippedItem = playerInventory.GetEquippedItem();
            if (equippedItem != null && !string.IsNullOrWhiteSpace(equippedItem.itemName))
                return $"{GetType().Name}:{equippedItem.itemName}";
        }

        return $"{GetType().Name}:{gameObject.name.Replace("(Clone)", "").Trim()}";
    }

    private GameObject GetSpawnPrefab()
    {
        if (particlePrefab1 != null)
            return particlePrefab1;
        return particlePrefab2;
    }

    private void SpawnHitPrefab(Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GetSpawnPrefab();
        if (prefab == null)
            return;

        GameObject spawned = Instantiate(prefab, position, rotation);
        if (particleLifetime > 0f)
            Destroy(spawned, particleLifetime);
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
        if (!Input.GetMouseButtonDown(1))
            return;

        if (!CanAttack())
        {
            Debug.Log($"Attack unavailable: equip {requiredEquippedItemName} first.");
            return;
        }

        if (WeaponCooldownManager.IsOnCooldown(cooldownKey, shootCooldown))
        {
            Debug.Log("Cooldown active, cannot shoot yet");
            return;
        }

        PlaySound(attackPressSound);
        WeaponCooldownManager.RecordUse(cooldownKey);
        Debug.Log("Step 1: Right mouse button pressed (cooldown passed)");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log("Step 2: Ray created from camera to mouse position");
        if (Raycast(ray.origin, ray.direction))
        {
            Debug.Log("Step 3: Raycast hit something: " + hit.collider.gameObject.name);
            SpawnHitPrefab(hit.point, Quaternion.LookRotation(hit.normal));

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

        }
        else
        {
            Debug.Log("Error: Raycast did not hit anything");
        }
    }
}
