using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("System References")]
    [Tooltip("XR Origin (AR placement)")]
    [SerializeField] private ARPlacementManager placementManager;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null) scoreText.text = "Score: " + GameManager.Instance.Score;
        if (highScoreText != null) highScoreText.text = "Best: " + GameManager.Instance.HighScore;

        if (timerText != null)
        {
            int timeLeft = Mathf.CeilToInt(GameManager.Instance.TimeRemaining);
            timerText.text = "Time: " + timeLeft;
        }

        if (GameManager.Instance.IsGameOver && gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Restart 
    public void RestartGame()
    {
        // Hide the Game Over Panel
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Reset the AR scanning
        if (placementManager != null) placementManager.ResetPlacement();

        // Reset the Game logic and clean up
        GameManager.Instance.SoftRestart();
    }
}