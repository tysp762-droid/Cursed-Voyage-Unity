using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPlayer : MonoBehaviour
{
    private HealthSystem healthSystem;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.OnDeath.AddListener(OnPlayerDeath);
        }
    }

    private void OnPlayerDeath()
    {
        SceneManager.LoadScene("DeathScene");
    }
}
