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

    private bool _isBoostActive = false;
    private bool _hasBoostTriggeredThisStage = false;
    private int _stage1FinalScore = 0;

    public bool IsBoostActive => _isBoostActive;

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
        _isBoostActive = false;
        _hasBoostTriggeredThisStage = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!HasGameStarted || IsGameOver || CurrentStage == GameStage.Intermission || _isBoostActive) return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            HandleTimeExpiration();
        }
    }

    private void HandleTimeExpiration()
    {
        if (CurrentStage == GameStage.Stage3)
        {
            GameOver();
        }
        else
        {
            // Check if threshold met
            if (StageScore >= CurrentThreshold())
            {
                StartIntermission();
            }
            else
            {
                Debug.Log($"Failed to reach threshold {CurrentThreshold()} in {CurrentStage}. Game Over.");
                GameOver();
            }
        }
    }

    public int CurrentThreshold()
    {
        switch (CurrentStage)
        {
            case GameStage.Stage1: return 50;
            case GameStage.Stage2: return _stage1FinalScore + 50;
            default: return int.MaxValue; // Stage 3 doesn't skip
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

        // Special case: Boost Trigger lantern
        if (lantern.type == LanternBehaviour.LanternType.Boost && !_isBoostActive)
        {
            ActivateBoostMode();
            return;
        }

        int points = 0;
        if (_isBoostActive)
        {
            points = 10;
        }
        else
        {
            if (lantern.type == LanternBehaviour.LanternType.Cursed)
                points = 5;
            else
                points = -1;
        }

        UpdateScore(points);
    }

    private void HandleLanternEscaped(LanternBehaviour lantern)
    {
        if (lantern == null || IsGameOver || CurrentStage == GameStage.Intermission) return;

        int points = 0;
        if (_isBoostActive)
        {
            points = 0; // No penalty during boost
        }
        else
        {
            if (lantern.type == LanternBehaviour.LanternType.Cursed)
            {
                points = -10;
                spawnerSystem.IncreaseCursedChance(curseChanceIncrease);
            }
            else if (lantern.type == LanternBehaviour.LanternType.Blessing)
            {
                points = 1;
            }
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
        // Don't advance during boost! Player plays full 10s.
        if (_isBoostActive) return;

        if (CurrentStage != GameStage.Stage3 && StageScore >= CurrentThreshold())
        {
            Debug.Log($"Threshold Met! Current Stage: {CurrentStage}, StageScore: {StageScore}");
            StartIntermission();
        }
    }

    public void ActivateBoostMode()
    {
        if (_hasBoostTriggeredThisStage) return;

        _isBoostActive = true;
        _hasBoostTriggeredThisStage = true;
        
        Debug.Log("BOOST MODE ACTIVATED!");
        spawnerSystem.ClearAllLanterns();
        StartCoroutine(BoostStateCoroutine());
    }

    private System.Collections.IEnumerator BoostStateCoroutine()
    {
        // Tell spawner to start specialized patterns
        spawnerSystem.StartBoostSpawning();

        float boostTimer = 10f;
        while (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            // We can add a specialized UI timer here later
            yield return null;
        }

        _isBoostActive = false;
        Debug.Log("Boost Mode Ended.");

        // After boost ends, check if we should leap immediately
        if (StageScore >= CurrentThreshold() && CurrentStage != GameStage.Stage3)
        {
            StartIntermission();
        }
    }

    private GameStage _lastPlayedStage;
    private void StartIntermission()
    {
        if (CurrentStage == GameStage.Stage1) _stage1FinalScore = StageScore;

        _lastPlayedStage = CurrentStage;
        CurrentStage = GameStage.Intermission;
        spawnerSystem.ClearAllLanterns();
        _hasBoostTriggeredThisStage = false; // Reset for next stage
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
        _isBoostActive = false;
        _hasBoostTriggeredThisStage = false;
        Time.timeScale = 1f;

        spawnerSystem.ClearAllLanterns();
        Debug.Log("Game State Reset.");
    }
}