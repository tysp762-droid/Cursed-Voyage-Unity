        using System.Collections.Generic;
using UnityEngine;

public class Shotgunhit : MonoBehaviour
{
    [Header("Cluster Attack")]
    [Tooltip("How far the shotgun cluster reaches.")]
    [SerializeField] private float clusterRange = 10f;
    [Tooltip("How many pellets are fired in the cluster.")]
    [SerializeField] private int pelletCount = 8;
    [Tooltip("How wide the cluster spread is in degrees.")]
    [SerializeField] [Range(0f, 90f)] private float spreadAngle = 20f;
    [Tooltip("Damage dealt to each enemy hit by the cluster.")]
    [SerializeField] private float pelletDamage = 20f;
    [Tooltip("Time in seconds between cluster attacks.")]
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private string cooldownIdentifier = "";
    [Tooltip("Which layers can be hit by the cluster.")]
    [SerializeField] private LayerMask hitLayerMask = ~0;
    [Tooltip("Inventory component used to check whether the shotgun is equipped.")]
    [SerializeField] private Inventory playerInventory;
    [Tooltip("Prefab required to be equipped for the cluster. This can be the hand prefab or item prefab.")]
    [SerializeField] private GameObject requiredEquippedPrefab;
    [Tooltip("Optional required equipped prefab name. Use this instead of a direct prefab reference if you want a name-based binding.")]
    [SerializeField] private string requiredEquippedPrefabName = "";
    [Tooltip("Optional required equipped item name. Use this if the equipped item name should gate the cluster.")]
    [SerializeField] private string requiredEquippedItemName = "";
    [Tooltip("Optional origin point for the cluster. If empty, uses this GameObject.")]
    [SerializeField] private Transform attackOrigin;
    [Tooltip("Input button used to trigger the cluster attack.")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse2;
    [Tooltip("Tag used to identify enemies if the target has that tag.")]
    [SerializeField] private string enemyTag = "Enemy";
    [Header("Audio")]
    [Tooltip("Optional AudioSource to play the shotgun sounds.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sound played when the shotgun fires.")]
    [SerializeField] private AudioClip fireSound;
    [Tooltip("Draw the cluster area in the editor.")]
    [SerializeField] private bool drawDebugGizmos = true;

    private string cooldownKey;

    private void Awake()
    {
        if (attackOrigin == null)
            attackOrigin = transform;

        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();

        if (attackKey == KeyCode.None)
            attackKey = KeyCode.Mouse2;

        if (playerInventory == null)
            Debug.LogWarning("Shotgunhit: No Inventory found in scene. Assign playerInventory or ensure an Inventory component exists.");

        cooldownKey = GetCooldownKey();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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

        AudioSource.PlayClipAtPoint(clip, transform.position);
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

    private void Start()
    {
        Debug.Log($"Shotgunhit: Start called on '{name}'. enabled={enabled}. attackOrigin={(attackOrigin != null ? attackOrigin.name : "null")}. inventory={(playerInventory != null ? playerInventory.name : "null")}.");
    }

    private bool CanFire(out string reason)
    {
        reason = null;

        if (playerInventory == null)
        {
            reason = "No Inventory assigned or found.";
            return false;
        }

        ItemsS0 equippedItem = playerInventory.GetEquippedItem();
        if (equippedItem == null)
        {
            reason = "No equipped item.";
            return false;
        }

        if (requiredEquippedPrefab != null)
        {
            if (IsMatchingPrefab(equippedItem.itemPrefab, requiredEquippedPrefab) || IsMatchingPrefab(equippedItem.handItemPrefab, requiredEquippedPrefab))
                return true;

            reason = $"Equipped item '{equippedItem.itemName}' does not match the required prefab '{requiredEquippedPrefab.name}'.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredEquippedPrefabName))
        {
            if (IsMatchingPrefabName(equippedItem.itemPrefab, requiredEquippedPrefabName) || IsMatchingPrefabName(equippedItem.handItemPrefab, requiredEquippedPrefabName))
                return true;

            reason = $"Equipped item '{equippedItem.itemName}' does not match the required prefab name '{requiredEquippedPrefabName}'.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredEquippedItemName))
        {
            if (string.Equals(equippedItem.itemName, requiredEquippedItemName, System.StringComparison.OrdinalIgnoreCase))
                return true;

            reason = $"Equipped item '{equippedItem.itemName}' does not match the required item name '{requiredEquippedItemName}'.";
            return false;
        }

        if (IsDefaultShotgunEquipped(equippedItem))
            return true;

        reason = $"Equipped item '{equippedItem.itemName}' is not a shotgun.";
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log($"Shotgunhit: attack button '{attackKey}' pressed.");

            if (!CanFire(out string reason))
            {
                Debug.Log($"Shotgunhit: attack blocked. {reason}");
                return;
            }

            if (WeaponCooldownManager.IsOnCooldown(cooldownKey, attackCooldown))
            {
                Debug.Log("Shotgunhit: attack on cooldown.");
                return;
            }

            WeaponCooldownManager.RecordUse(cooldownKey);
            Debug.Log("Shotgunhit: cluster fire executed.");
            PlaySound(fireSound);
            FireCluster();
        }
    }

    private void FireCluster()
    {
        Vector3 origin = attackOrigin.position;
        Vector3 forward = attackOrigin.forward;
        HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

        for (int i = 0; i < pelletCount; i++)
        {
            float yaw = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            float pitch = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector3 pelletDirection = Quaternion.Euler(pitch, yaw, 0f) * forward;

            if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, clusterRange, hitLayerMask))
            {
                Debug.DrawLine(origin, hit.point, Color.yellow, 1f);
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy == null)
                    continue;

                if (!string.IsNullOrEmpty(enemyTag) && !hit.collider.CompareTag(enemyTag) && !enemy.CompareTag(enemyTag))
                    continue;

                if (hitEnemies.Contains(enemy))
                    continue;

                enemy.DoDmg(pelletDamage);
                hitEnemies.Add(enemy);
                Debug.Log($"Shotgunhit: hit enemy {enemy.name} for {pelletDamage} damage.");
            }
            else
            {
                Debug.DrawRay(origin, pelletDirection * clusterRange, Color.red, 1f);
            }
        }

        Debug.Log($"Shotgunhit: fired {pelletCount} pellets, hit {hitEnemies.Count} enemies.");
    }

    private bool IsMatchingPrefab(GameObject prefab, GameObject requiredPrefab)
    {
        if (prefab == null || requiredPrefab == null)
            return false;

        if (prefab == requiredPrefab)
            return true;

        return string.Equals(prefab.name, requiredPrefab.name, System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsMatchingPrefabName(GameObject prefab, string requiredPrefabName)
    {
        return prefab != null && string.Equals(prefab.name, requiredPrefabName, System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsDefaultShotgunEquipped(ItemsS0 equippedItem)
    {
        if (equippedItem == null)
            return false;

        return HasItemNameKeyword(equippedItem, "shotgun", "gun", "blaster");
    }

    private bool HasItemNameKeyword(ItemsS0 equippedItem, params string[] keywords)
    {
        if (equippedItem == null)
            return false;

        if (!string.IsNullOrWhiteSpace(equippedItem.itemName) && ContainsKeyword(equippedItem.itemName, keywords))
            return true;

        if (equippedItem.itemPrefab != null && ContainsKeyword(equippedItem.itemPrefab.name, keywords))
            return true;

        if (equippedItem.handItemPrefab != null && ContainsKeyword(equippedItem.handItemPrefab.name, keywords))
            return true;

        return false;
    }

    private bool ContainsKeyword(string value, params string[] keywords)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        foreach (string keyword in keywords)
        {
            if (value.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos)
            return;

        Transform origin = attackOrigin != null ? attackOrigin : transform;
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawWireSphere(origin.position, clusterRange);

        Vector3 forward = origin.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin.position, origin.position + forward * clusterRange);

        for (int i = 0; i < Mathf.Min(pelletCount, 16); i++)
        {
            float yaw = Mathf.Lerp(-spreadAngle * 0.5f, spreadAngle * 0.5f, i / (float)(Mathf.Max(pelletCount - 1, 1)));
            float pitch = Mathf.Lerp(-spreadAngle * 0.5f, spreadAngle * 0.5f, i / (float)(Mathf.Max(pelletCount - 1, 1)));
            Vector3 direction = Quaternion.Euler(pitch, yaw, 0f) * forward;
            Gizmos.DrawLine(origin.position, origin.position + direction * clusterRange);
        }
    }
}
