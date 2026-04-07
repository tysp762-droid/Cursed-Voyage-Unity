using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public int highScore = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Laad de opgeslagen high score bij het starten
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    public void AddPoint()
    {
        score++;
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        // Update high score als huidige score hoger is
        if (score > highScore)
        {
            highScore = score;
            UpdateHighScoreText();

            // Sla de nieuwe high score op
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }
}
