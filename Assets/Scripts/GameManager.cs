using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameTime = 30f; // total game duration
    [SerializeField] private float curseChanceIncrease = 0.05f;

    [Header("System References")]
    [SerializeField] private SpawnerSystem spawnerSystem;

    // Public properties for UI Manager
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool HasGameStarted { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        TimeRemaining = gameTime;
        IsGameOver = false;
        HasGameStarted = false; 
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Countdown when game start
        if (!HasGameStarted || IsGameOver) return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            GameOver();
        }
    }

    // called by AR Placement script
    public void StartGame()
    {
        HasGameStarted = true;
        Debug.Log("Game Started! Timer is running.");
    }

    void OnEnable()
    {
        LanternBehaviour.OnLanternHit += HandleLanternHit;
        LanternBehaviour.OnLanternEscaped += HandleLanternEscaped;
    }

    void OnDisable()
    {
        LanternBehaviour.OnLanternHit -= HandleLanternHit;
        LanternBehaviour.OnLanternEscaped -= HandleLanternEscaped;
    }

    private void HandleLanternHit(LanternBehaviour lantern)
    {
        if (lantern == null || IsGameOver) return;

        if (lantern.type == LanternBehaviour.LanternType.Cursed)
            Score += 5;
        else
            Score -= 1;

        Score = Mathf.Max(0, Score);
    }

    private void HandleLanternEscaped(LanternBehaviour lantern)
    {
        if (lantern == null || IsGameOver) return;

        if (lantern.type == LanternBehaviour.LanternType.Cursed)
        {
            Score -= 10;
            spawnerSystem.IncreaseCursedChance(curseChanceIncrease);
        }
        else if (lantern.type == LanternBehaviour.LanternType.Blessing)
        {
            Score += 1;
        }

        Score = Mathf.Max(0, Score);
    }

    private void GameOver()
    {
        IsGameOver = true;
        Debug.Log("GAME OVER. Final Score: " + Score);

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
            Debug.Log("NEW HIGH SCORE: " + HighScore);
        }

        Time.timeScale = 0f;
    }
    public void SoftRestart()
    {
        Score = 0;
        TimeRemaining = gameTime; // Reset to 30s
        IsGameOver = false;
        HasGameStarted = false;
        Time.timeScale = 1f;

        // Tell the spawner to clean up the sky
        spawnerSystem.ClearAllLanterns();

        Debug.Log("Game State Reset.");
    }
}