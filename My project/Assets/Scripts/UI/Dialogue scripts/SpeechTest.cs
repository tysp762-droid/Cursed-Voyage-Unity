using UnityEngine;
using TMPro; // Voor TextMeshPro

public class SpeechTest : MonoBehaviour
{
    // RectTransform van de tekstwolk UI (om de tekstwolk te kunnen tonen of verbergen)
    public RectTransform speechBubbleRect;

    // De tekstcomponent in de tekstwolk
    public TextMeshProUGUI textMeshPro;

    
    // Functie om tekst te zetten en tekstwolk te tonen/verbergen
    public void SetText(string message)
    {
        if (textMeshPro != null && speechBubbleRect != null)
        {
            textMeshPro.text = message;

            // Toon de tekstwolk alleen als er tekst is, anders verberg
            speechBubbleRect.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }
}
