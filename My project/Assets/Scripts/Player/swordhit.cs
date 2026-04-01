using System.Collections.Generic;
using UnityEngine;

public class swordhit : MonoBehaviour
{
    [Header("Sweep Attack")]
    [Tooltip("How far the sword sweep reaches.")]
    [SerializeField] private float sweepRange = 2f;
    [Tooltip("How wide the sweep cone is in degrees.")]
    [SerializeField] [Range(10f, 180f)] private float sweepAngle = 90f;
    [Tooltip("Damage dealt to each enemy hit by the sweep.")]
    [SerializeField] private float sweepDamage = 25f;
    [Tooltip("Time in seconds between sweep attacks.")]
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private string cooldownIdentifier = "";
    [Tooltip("Which layers can be hit by the sweep.")]
    [SerializeField] private LayerMask hitLayerMask = ~0;
    [Tooltip("Inventory component used to check whether the sword is equipped.")]
    [SerializeField] private Inventory playerInventory;
    [Tooltip("Prefab required to be equipped for the sweep. This can be the hand prefab or item prefab.")]
    [SerializeField] private GameObject requiredEquippedPrefab;
    [Tooltip("Optional required equipped prefab name. Use this instead of a direct prefab reference if you want a name-based binding.")]
    [SerializeField] private string requiredEquippedPrefabName = "";
    [Tooltip("Optional required equipped item name. Use this if the equipped item name should gate the sweep.")]
    [SerializeField] private string requiredEquippedItemName = "";
    [Tooltip("Optional origin point for the sweep. If empty, uses this GameObject.")]
    [SerializeField] private Transform attackOrigin;
    [Tooltip("Input button used to trigger the sweep attack.")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [Tooltip("Tag used to identify enemies if the target has that tag.")]
    [SerializeField] private string enemyTag = "Enemy";
    [Header("Audio")]
    [Tooltip("Optional AudioSource to play the sword sounds.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sound played when the sword swing button is pressed.")]
    [SerializeField] private AudioClip swingSound;
    [Tooltip("Draw the sweep area in the editor.")]
    [SerializeField] private bool drawDebugGizmos = true;

    private string cooldownKey;

    private void Awake()
    {
        if (attackOrigin == null)
            attackOrigin = transform;

        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();

        if (attackKey == KeyCode.None)
            attackKey = KeyCode.Mouse0;

        if (playerInventory == null)
            Debug.LogWarning("swordhit: No Inventory found in scene. Assign playerInventory or ensure an Inventory component exists.");

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
        Debug.Log($"swordhit: Start called on '{name}'. enabled={enabled}. attackOrigin={(attackOrigin != null ? attackOrigin.name : "null")}. inventory={(playerInventory != null ? playerInventory.name : "null")}.");
    }

    private bool CanSweep(out string reason)
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
            if (IsMatchingPrefab(equippedItem.handItemPrefab, requiredEquippedPrefab) || IsMatchingPrefab(equippedItem.itemPrefab, requiredEquippedPrefab))
                return true;

            reason = $"Equipped item '{equippedItem.itemName}' does not match the required prefab '{requiredEquippedPrefab.name}'.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredEquippedPrefabName))
        {
            if (IsMatchingPrefabName(equippedItem.handItemPrefab, requiredEquippedPrefabName) || IsMatchingPrefabName(equippedItem.itemPrefab, requiredEquippedPrefabName))
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

        if (IsDefaultSwordEquipped(equippedItem))
            return true;

        reason = $"Equipped item '{equippedItem.itemName}' is not a sword.";
        return false;
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

    private bool IsDefaultSwordEquipped(ItemsS0 equippedItem)
    {
        if (equippedItem == null)
            return false;

        return HasItemNameKeyword(equippedItem, "sword", "cutlass", "blade");
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

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log($"swordhit: attack button '{attackKey}' pressed.");

            if (!CanSweep(out string reason))
            {
                Debug.Log($"swordhit: attack blocked. {reason}");
                return;
            }

            if (WeaponCooldownManager.IsOnCooldown(cooldownKey, attackCooldown))
            {
                Debug.Log("swordhit: attack on cooldown.");
                return;
            }

            WeaponCooldownManager.RecordUse(cooldownKey);
            Debug.Log("swordhit: swing executed.");
            PlaySound(swingSound);
            SweepAttack();
        }
    }

    private void SweepAttack()
    {
        Vector3 origin = attackOrigin.position;
        Vector3 forward = attackOrigin.forward;
        Collider[] hits = Physics.OverlapSphere(origin, sweepRange, hitLayerMask);

        Debug.DrawRay(origin, forward * sweepRange, Color.red, 1f);
        Debug.Log($"swordhit: sweep origin={origin}, forward={forward}, hits={hits.Length}");

        HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || hitEnemies.Contains(enemy))
                continue;

            if (!string.IsNullOrEmpty(enemyTag) && !hit.CompareTag(enemyTag) && !enemy.CompareTag(enemyTag))
                continue;

            Vector3 directionToTarget = (hit.transform.position - origin).normalized;
            float angleToTarget = Vector3.Angle(forward, directionToTarget);

            Debug.DrawLine(origin, hit.ClosestPoint(origin), Color.yellow, 1f);
            Debug.Log($"swordhit: candidate={hit.name}, angle={angleToTarget:F1}, distance={(hit.transform.position - origin).magnitude:F2}");

            if (angleToTarget <= sweepAngle * 0.5f)
            {
                enemy.DoDmg(sweepDamage);
                hitEnemies.Add(enemy);
                Debug.Log($"swordhit: hit enemy {enemy.name} for {sweepDamage} damage.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos)
            return;

        Transform origin = attackOrigin != null ? attackOrigin : transform;
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(origin.position, sweepRange);

        Vector3 forward = origin.forward;
        float halfAngle = sweepAngle * 0.5f * Mathf.Deg2Rad;
        Vector3 rightRay = Quaternion.AngleAxis(sweepAngle * 0.5f, origin.up) * forward;
        Vector3 leftRay = Quaternion.AngleAxis(-sweepAngle * 0.5f, origin.up) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position, origin.position + rightRay * sweepRange);
        Gizmos.DrawLine(origin.position, origin.position + leftRay * sweepRange);
    }
}
