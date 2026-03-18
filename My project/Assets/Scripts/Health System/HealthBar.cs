using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private HealthSystem healthSystem;
    private TMP_Text healthText; // This script should be on the TMP_Text object

    void Start()
    {
        healthText = GetComponent<TMP_Text>(); // This object must have TMP_Text
        healthSystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<HealthSystem>(); // Find HealthSystem on Player
        Debug.Log("Found HealthSystem: " + (healthSystem != null ? healthSystem.gameObject.name : "null"));
        if (healthText != null && healthSystem != null)
        {
            Debug.Log("TMP_Text and HealthSystem found, subscribing to OnHealthChanged");
            healthSystem.OnHealthChanged.AddListener(UpdateHealthText);
            // Initialize the text with current health percentage
            UpdateHealthText(healthSystem.GetHealthPercentage());
        }
        else
        {
            if (healthText == null) Debug.LogError("TMP_Text component not found on this object! Attach this script to the TMP_Text GameObject.");
            if (healthSystem == null) Debug.LogError("No HealthSystem found in the scene!");
        }
    }

    // void Update()
    // {
    //     // Test if TMP_Text updates at all
    //     if (healthText != null)
    //     {
    //         healthText.text = "Test: " + Time.time.ToString("F1");
    //     }
    // }

    private void UpdateHealthText(float healthPercentage)
    {
        Debug.Log("Updating health text to: " + healthPercentage + "%");
        if (healthText != null)
        {
            string newText = "Health: " + healthPercentage.ToString("F0") + "%";
            healthText.text = newText;
            Debug.Log("Set TMP_Text to: " + newText + ", current text is: " + healthText.text);
        }
        else
        {
            Debug.LogError("HealthText is null!");
        }
    }
}
