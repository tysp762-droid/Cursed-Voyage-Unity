using UnityEngine;
using UnityEngine.SceneManagement;

public class YapManeger : MonoBehaviour
{
    // Referentie naar het SpeechTest script dat de tekst op het scherm toont
    [SerializeField] private SpeechTest speechBubble;

    // Naam van de volgende scène die geladen wordt als de dialoog klaar is
    [SerializeField] private string nextSceneName;

    // Lijst met alle dialoogregels die achter elkaar getoond worden
    private string[] dialogueLines = 
    {
        "Arr... ye found me...",
        "Good... good...",
        "Listen close, matey...",
        "I ain't got much time...",
        "The ship... she is gone...",
        "To pieces... but there is hope...",
        "Cough... groan...",
        "you have to survive...",
        "now find some weapons... and fight for your life...",
        "I wish I could help you more... but I am too weak...",
        "Go... now... before the tribe find us both...",
        " Cough... groan... "
    };

    // Houdt bij welke dialoogregel als volgende getoond moet worden
    private int currentLine = 0;

    //  wordt één keer uitgevoerd als het script begint
    void Start()
    {
        // Toon meteen de eerste dialoogregel
        ShowNextLine();
    }
    void Update()
    {
        // Controleer of de speler op de spatiebalk drukt
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toon de volgende dialoogregel als er op spatie is gedrukt
            ShowNextLine();
        }
    }

    // Methode om de volgende dialoogregel te tonen of de scène te wisselen als het einde is bereikt
    public void ShowNextLine()
    {
        // Als er nog dialoogregels over zijn
        if (currentLine < dialogueLines.Length)
        {
            // Zet de tekst van de huidige dialoogregel in de tekstwolk via het SpeechTest script
            speechBubble.SetText(dialogueLines[currentLine]);

            // Verhoog de teller zodat de volgende regel bij de volgende keer getoond wordt
            currentLine++;
        }
        else
        {
            // Als alle dialoogregels getoond zijn, laad de volgende scène
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
