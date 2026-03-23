using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddPoint()
    {
        score++;
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}