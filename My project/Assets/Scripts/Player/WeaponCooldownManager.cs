using System.Collections.Generic;
using UnityEngine;

public static class WeaponCooldownManager
{
    private static readonly Dictionary<string, float> lastUseTimes = new Dictionary<string, float>();

    public static bool IsOnCooldown(string key, float cooldown)
    {
        if (string.IsNullOrWhiteSpace(key) || cooldown <= 0f)
            return false;

        if (lastUseTimes.TryGetValue(key, out float lastUseTime))
            return Time.time < lastUseTime + cooldown;

        return false;
    }

    public static void RecordUse(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        lastUseTimes[key] = Time.time;
    }

    public static float GetRemainingCooldown(string key, float cooldown)
    {
        if (string.IsNullOrWhiteSpace(key) || cooldown <= 0f)
            return 0f;

        if (lastUseTimes.TryGetValue(key, out float lastUseTime))
        {
            float remaining = (lastUseTime + cooldown) - Time.time;
            return remaining > 0f ? remaining : 0f;
        }

        return 0f;
    }
}
