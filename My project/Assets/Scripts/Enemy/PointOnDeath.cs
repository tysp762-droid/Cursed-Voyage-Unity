using UnityEngine;

public class PointOnDeath : MonoBehaviour
{
    // This runs automatically when Destroy(gameObject) is called by ANY script
    private void OnDestroy()
    {
        // 1. Check if the application is actually playing 
        // (prevents errors when you stop the game in the editor)
        if (!gameObject.scene.isLoaded) return;

        // 2. Check the Tag
        if (gameObject.CompareTag("Enemy"))
        {
            // 3. Send the point to the Manager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoint();
            }
        }
    }
}