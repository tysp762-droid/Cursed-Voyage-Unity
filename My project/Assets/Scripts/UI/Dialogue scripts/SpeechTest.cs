using UnityEngine;
using TMPro; // Als je TextMeshPro gebruikt

public class SpeechTest : MonoBehaviour
{
    public Transform character; // Het karakter in de wereld
    public Vector3 offset = new Vector3(0, 2, 0); // Offset boven het hoofd in wereldruimte
    public RectTransform speechBubbleRect; // RectTransform van de tekstwolk UI
    public TextMeshProUGUI textMeshPro; // De tekstcomponent in de tekstwolk

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (character != null && speechBubbleRect != null && textMeshPro != null)
        {
            // Wereldpositie + offset
            Vector3 worldPosition = character.position + offset;

            // Zet wereldpositie om naar schermpositie (pixels)
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

            // Controleer of het karakter in beeld is (z > 0 betekent voor de camera)
            if (screenPosition.z > 0 && !string.IsNullOrEmpty(textMeshPro.text))
            {
                // Zet de positie van de UI tekstwolk
                speechBubbleRect.position = screenPosition;
                speechBubbleRect.gameObject.SetActive(true);
            }
            else
            {
                // Verberg de tekstwolk als het karakter niet in beeld is of tekst leeg is
                speechBubbleRect.gameObject.SetActive(false);
            }
        }
    }

    // Functie om tekst te zetten en tekstwolk te tonen/verbergen
    public void SetText(string message)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = message;

            // Verberg tekstwolk als tekst leeg is
            speechBubbleRect.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }
}
