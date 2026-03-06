using UnityEngine;

public class YapManager : MonoBehaviour
{
    [SerializeField] private SpeechTest speechBubble; // Verwijzing naar jouw SpeechTest script

    private string[] dialogueLines = {
        "Arr... ye found me...",
        "Good... good...",
        "Listen close, matey...",
        "I ain't got much time...",
        "The ship... she is gone...",
        "To pieces... but there  is hope...",
        "Cough... groan..."
    };

    private int currentLine = 0;

    void Start()
    {
        if (speechBubble == null)
        {
            Debug.LogError("SpeechBubble reference not set on " + name);
            enabled = false;
            return;
        }

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
            speechBubble.SetText(""); // Verberg tekstwolk als dialoog klaar is
        }
    }
}
