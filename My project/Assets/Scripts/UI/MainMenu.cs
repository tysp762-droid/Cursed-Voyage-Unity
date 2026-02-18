using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    
    private NextScene nextSceneManager;

    private void Start()
    {
        // Get the NextScene component from this GameObject or create it
        nextSceneManager = GetComponent<NextScene>();
        if (nextSceneManager == null)
        {
            nextSceneManager = gameObject.AddComponent<NextScene>();
        }

        // Assign button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        nextSceneManager.LoadScene("Gameplay");
    }

    private void OnQuitButtonClicked()
    {
        nextSceneManager.QuitGame();
    }
}
