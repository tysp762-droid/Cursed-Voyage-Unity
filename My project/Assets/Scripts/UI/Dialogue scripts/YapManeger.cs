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
        "FYI this is a placeholder dialogue, it will be replaced with the actual dialogue later on",
        "this is my first time writing a dialogue, so it might be a bit rough, but",
        "it is also my first game, so I hope you can forgive me for that",
        "here is the actual dialogue:",
        "Arr... ye found me...",
        "Good... good...",
        "Listen close, matey...",
        "we are in a bad spot... the tribe is after us both...",
        " our ship was attacked... and I got hit...",
        "then you passed out... and woke up here... on this island...",
        "all Of our crew is either dead or captured",
        "... and I am too weak to fight...",
        "but you... you are still alive...",
        "Cough... groan...",
        " there should be some supplies around here...",
        "but I am too weak to get them... you have to find them for me...",   
        "there should be some rum around here...",
        "there might also be a weapon or two around here...",   
        "our chests are probably scattered around here somewhere",
        "... they might have some supplies in them...",
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
