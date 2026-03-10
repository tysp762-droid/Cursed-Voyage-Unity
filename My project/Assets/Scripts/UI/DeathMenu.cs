using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assign button click listeners
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRetryClicked()
    {
        // Load IslandScene when Retry is clicked
        SceneManager.LoadScene("IslandScene");
    }

    private void OnQuitClicked()
    {
        // Load MainMenu when Quit is clicked
        SceneManager.LoadScene("MainMenu");
    }
}
