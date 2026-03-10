using UnityEngine;

public class YapManeger : MonoBehaviour
{
    [SerializeField] private SpeechTest speechBubble; // Verwijzing naar jouw SpeechTest script

    private string[] dialogueLines = {
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
            speechBubble.SetText(""); // Verberg tekstwolk als dialoog klaar is
        }
    }
}

