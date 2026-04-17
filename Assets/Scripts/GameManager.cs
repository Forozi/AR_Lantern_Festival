using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameStage { Stage1, Stage2, Stage3, Intermission }

    [Header("Game Settings")]
    [SerializeField] private float stageDuration = 30f;
    [SerializeField] private int stageSkipThreshold = 50;
    [SerializeField] private float curseChanceIncrease = 0.05f;

    [Header("System References")]
    [SerializeField] private SpawnerSystem spawnerSystem;

    // Public properties for UI Manager
    public int Score { get; private set; }
    public int StageScore { get; private set; }
    public int HighScore { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool HasGameStarted { get; private set; }
    public GameStage CurrentStage { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        TimeRemaining = stageDuration;
        IsGameOver = false;
        HasGameStarted = false; 
        CurrentStage = GameStage.Stage1;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!HasGameStarted || IsGameOver || CurrentStage == GameStage.Intermission) return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            if (CurrentStage == GameStage.Stage3)
            {
                GameOver();
            }
            else
            {
                StartIntermission();
            }
        }
    }

    public void StartGame()
    {
        HasGameStarted = true;
        Debug.Log("Game Started! Stage: " + CurrentStage);
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
        if (lantern == null || IsGameOver || CurrentStage == GameStage.Intermission) return;

        int points = 0;
        if (lantern.type == LanternBehaviour.LanternType.Cursed)
            points = 5;
        else
            points = -1;

        UpdateScore(points);
    }

    private void HandleLanternEscaped(LanternBehaviour lantern)
    {
        if (lantern == null || IsGameOver || CurrentStage == GameStage.Intermission) return;

        int points = 0;
        if (lantern.type == LanternBehaviour.LanternType.Cursed)
        {
            points = -10;
            spawnerSystem.IncreaseCursedChance(curseChanceIncrease);
        }
        else if (lantern.type == LanternBehaviour.LanternType.Blessing)
        {
            points = 1;
        }

        UpdateScore(points);
    }

    private void UpdateScore(int points)
    {
        Score += points;
        StageScore += points;

        Score = Mathf.Max(0, Score);
        StageScore = Mathf.Max(0, StageScore);

        CheckStageProgression();
    }

    private void CheckStageProgression()
    {
        if (CurrentStage != GameStage.Stage3 && StageScore >= stageSkipThreshold)
        {
            Debug.Log($"Threshold Met! Current Stage: {CurrentStage}, StageScore: {StageScore}");
            StartIntermission();
        }
    }

    private GameStage _lastPlayedStage;
    private void StartIntermission()
    {
        _lastPlayedStage = CurrentStage;
        CurrentStage = GameStage.Intermission;
        spawnerSystem.ClearAllLanterns();
        StartCoroutine(IntermissionCoroutine());
    }

    private System.Collections.IEnumerator IntermissionCoroutine()
    {
        Debug.Log("Intermission Started...");
        yield return new WaitForSeconds(3f); // 3-2-1 Countdown placeholder
        AdvanceStage();
    }
    private void AdvanceStage()
    {
        if (CurrentStage == GameStage.Intermission)
        {
            if (_lastPlayedStage == GameStage.Stage1) CurrentStage = GameStage.Stage2;
            else if (_lastPlayedStage == GameStage.Stage2) CurrentStage = GameStage.Stage3;
            else CurrentStage = GameStage.Stage1; // Fallback
        }

        TimeRemaining = stageDuration;
        StageScore = 0;
        Debug.Log("Advanced to: " + CurrentStage);
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
        StageScore = 0;
        TimeRemaining = stageDuration;
        IsGameOver = false;
        HasGameStarted = false;
        CurrentStage = GameStage.Stage1;
        Time.timeScale = 1f;

        spawnerSystem.ClearAllLanterns();
        Debug.Log("Game State Reset.");
    }
}