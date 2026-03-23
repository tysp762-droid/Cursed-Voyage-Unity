using UnityEngine;
using UnityEngine.SceneManagement;

public class YapManeger : MonoBehaviour
{
    [SerializeField] private SpeechTest speechBubble; // Verwijzing naar jouw SpeechTest script
    [SerializeField] private string nextSceneName; // Name of the scene to load after dialogue

    private string[] dialogueLines = 
    {
        "Arr... ye found me...",
        "Good... good...",
        "Listen close, matey...",
        "I ain't got much time...",
        "The ship... she is gone...",
        "To pieces... but there  is hope...",
        "Cough... groan...",
        "you have to survive...",
        "now find some weapons... and fight for your life...",
        "I wish I could help you more... but I am too weak...",
        "Fuck you matey",
        " *dies of cringe thinking about you*",  

    };

    private int currentLine = 0;

    void Start()
    {
        ShowNextLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    public void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            speechBubble.SetText(dialogueLines[currentLine]);
            currentLine++;
        }
        else
        {
            // Dialogue ended, load the next scene
            LoadScene(nextSceneName);
        }
    }

        /// <summary>
    /// Loads the next scene by name
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

