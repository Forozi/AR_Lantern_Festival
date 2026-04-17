using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnerSystem : MonoBehaviour
{
    public class SpawnPoint
    {
        public Vector3 position;
        public LanternBehaviour occupiedBy = null;
        public bool IsOccupied => occupiedBy != null;
    }

    [Header("References")]
    public Transform spawnCenterAnchor; // Will be set by AR placement
    [SerializeField] private LanternBehaviour lanternPrefab;

    [Header("Visuals")]
    [SerializeField] private Material cursedMaterial;
    [SerializeField] private Material blessedMaterial;
    [SerializeField] private Material boostMaterial;

    [Header("Spawn Config")]
    [SerializeField] private int numberOfSpawnPoints = 12;
    [SerializeField] private float spawnRadius = 3f; // Reduced slightly for AR room testing
    [SerializeField] private float spawnHeightOffset = -0.5f; // Closer to floor
    [SerializeField, Range(0, 1)] private float cursedLanternChance = 0.7f;
    [SerializeField] private float spawnOffsetRadius = 0.5f;

    [Header("Timing")]
    [SerializeField] private float minSpawnTime = 0.5f;
    [SerializeField] private float maxSpawnTime = 1.0f;
    //[SerializeField] private float secondWaveDelay = 2f;

    private float _spawnTimer;
    private bool _isInitialized = false;

    private List<SpawnPoint> _spawnPoints = new();
    private Vector3[] _spawnPointOffsets;
    private IObjectPool<LanternBehaviour> _lanternPool;

    private List<SpawnPoint> _shuffledSpawnQueue = new();
    private int _currentSpawnIndex = 0;
    
    [Header("Wave Config")]
    [Tooltip("How many seconds between each big wave")]
    [SerializeField] private float waveInterval = 5.0f;
    [Tooltip("How many lanterns spawn at the exact same time in a wave")]
    [SerializeField] private int lanternsPerWave = 4;
    [Tooltip("Bonus spawns when a lantern escapes/is shot")]
    [SerializeField] private int revengeSpawns = 1;

    private float _waveTimer;
    void Awake()
    {
        // Increase pool max to 60 for multi-row boost patterns
        _lanternPool = new ObjectPool<LanternBehaviour>(
            CreateLantern, null, OnReleaseLantern, OnDestroyLantern, true, 20, 60
        );

        _spawnPointOffsets = new Vector3[numberOfSpawnPoints];
        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            float angle = i * (360f / numberOfSpawnPoints);
            float rad = angle * Mathf.Deg2Rad;

            float x = spawnRadius * Mathf.Cos(rad);
            float z = spawnRadius * Mathf.Sin(rad);
            _spawnPointOffsets[i] = new Vector3(x, spawnHeightOffset, z);

            _spawnPoints.Add(new SpawnPoint());
        }
    }

    void OnEnable()
    {
        LanternBehaviour.OnLanternHit += HandleLanternEvent;
        LanternBehaviour.OnLanternEscaped += HandleLanternEvent;
    }

    void OnDisable()
    {
        LanternBehaviour.OnLanternHit -= HandleLanternEvent;
        LanternBehaviour.OnLanternEscaped -= HandleLanternEvent;
    }

    // === NEW: Called by AR Script once the player taps the floor ===
    public void SetAnchorAndInitialize(Transform newAnchor)
    {
        spawnCenterAnchor = newAnchor;

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            _spawnPoints[i].position = spawnCenterAnchor.position + _spawnPointOffsets[i];
        }

        // Trigger the very first wave immediately!
        _waveTimer = 0f;
        ResetAndShuffleQueue();

        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized || GameManager.Instance.IsGameOver) return;

        // Skip normal spawning during boost mode
        if (GameManager.Instance.IsBoostActive) return;

        _waveTimer -= Time.deltaTime;

        // Every 5 seconds, spawn a batch!
        if (_waveTimer <= 0)
        {
            SpawnBatch(lanternsPerWave);
            _waveTimer = waveInterval; // Reset timer for the next 5 seconds
        }

        CheckForBoostGuarantee();
    }

    private void CheckForBoostGuarantee()
    {
        // 20s mark guarantee (Timer starts at 30s, so check when it reaches 10s)
        if (Mathf.Abs(GameManager.Instance.TimeRemaining - 10.0f) < 0.1f)
        {
            if (GameManager.Instance.StageScore < GameManager.Instance.CurrentThreshold())
            {
                // Force a boost lantern if one hasn't spawned yet
                SpawnBoostTrigger();
            }
        }
    }

    private void SpawnBoostTrigger()
    {
        // Try to find a free point to force the trigger
        foreach (var point in _spawnPoints)
        {
            if (!point.IsOccupied)
            {
                SpawnAt(point, forceBoost: true);
                return;
            }
        }
    }

    public void IncreaseCursedChance(float amount)
    {
        cursedLanternChance = Mathf.Clamp01(cursedLanternChance + amount);
    }

    private void TrySpawnLantern()
    {
        for (int i = 0; i < _shuffledSpawnQueue.Count; i++)
        {
            if (_currentSpawnIndex >= _shuffledSpawnQueue.Count)
                ResetAndShuffleQueue();

            SpawnPoint pointToTry = _shuffledSpawnQueue[_currentSpawnIndex];
            _currentSpawnIndex++;

            if (!pointToTry.IsOccupied)
            {
                SpawnAt(pointToTry);
                return;
            }
        }
    }

    private void ResetAndShuffleQueue()
    {
        if (_shuffledSpawnQueue.Count == 0)
            _shuffledSpawnQueue.AddRange(_spawnPoints);

        // Simple shuffle algorithm
        for (int i = 0; i < _shuffledSpawnQueue.Count; i++)
        {
            int rand = Random.Range(i, _shuffledSpawnQueue.Count);
            (_shuffledSpawnQueue[i], _shuffledSpawnQueue[rand]) = (_shuffledSpawnQueue[rand], _shuffledSpawnQueue[i]);
        }
        _currentSpawnIndex = 0;
    }

    private void SpawnAt(SpawnPoint point, bool forceBoost = false)
    {
        bool isBoost = forceBoost || (Random.value < GetBoostChanceForStage());
        
        LanternBehaviour.LanternType type;
        Material mat;

        if (isBoost)
        {
            type = LanternBehaviour.LanternType.Boost;
            mat = boostMaterial;
        }
        else
        {
            bool isCursed = Random.value < cursedLanternChance;
            type = isCursed ? LanternBehaviour.LanternType.Cursed : LanternBehaviour.LanternType.Blessing;
            mat = isCursed ? cursedMaterial : blessedMaterial;
        }

        LanternBehaviour lantern = _lanternPool.Get();
        Vector2 offset2D = Random.insideUnitCircle * spawnOffsetRadius;

        lantern.transform.position = point.position + new Vector3(offset2D.x, 0f, offset2D.y);
        point.occupiedBy = lantern;

        lantern.currentSpeedMultiplier = GetSpeedMultiplierForStage();
        lantern.Initialize(type, mat, _lanternPool, point);
    }

    private float GetBoostChanceForStage()
    {
        if (GameManager.Instance == null) return 0.01f;
        
        switch (GameManager.Instance.CurrentStage)
        {
            case GameManager.GameStage.Stage2: return 0.008f;
            case GameManager.GameStage.Stage3: return 0.006f;
            default: return 0.01f;
        }
    }

    public void StartBoostSpawning()
    {
        _isInitialized = true; // Ensure we are active
        StopAllCoroutines();
        StartCoroutine(BoostSpawningCoroutine());
    }

    private IEnumerator BoostSpawningCoroutine()
    {
        int rowCount = 2;
        if (GameManager.Instance.CurrentStage == GameManager.GameStage.Stage2) rowCount = 3;
        else if (GameManager.Instance.CurrentStage == GameManager.GameStage.Stage3) rowCount = 4;

        float rowHeightGap = 0.5f;

        // Immediately spawn X rows
        for (int r = 0; r < rowCount; r++)
        {
            SpawnBoostRow(r * rowHeightGap);
            yield return new WaitForSeconds(0.2f); // Slight stagger for visual effect
        }
    }

    private void SpawnBoostRow(float heightOffset)
    {
        foreach (var point in _spawnPoints)
        {
            LanternBehaviour lantern = _lanternPool.Get();
            Vector2 ringOffset = Random.insideUnitCircle * 0.2f; // Tighter ring
            
            Vector3 spawnPos = point.position + new Vector3(ringOffset.x, heightOffset, ringOffset.y);
            lantern.transform.position = spawnPos;
            
            // Note: In Boost mode, we don't occupy spawn points strictly to allow overlapping rings
            lantern.currentSpeedMultiplier = GetSpeedMultiplierForStage();
            lantern.Initialize(LanternBehaviour.LanternType.Boost, boostMaterial, _lanternPool, null);
        }
    }

    private float GetSpeedMultiplierForStage()
    {
        if (GameManager.Instance == null) return 1.0f;

        switch (GameManager.Instance.CurrentStage)
        {
            case GameManager.GameStage.Stage2: return 1.5f;
            case GameManager.GameStage.Stage3: return 2.5f;
            default: return 1.0f;
        }
    }

    private void ResetSpawnTimer() => _spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

    private LanternBehaviour CreateLantern() => Instantiate(lanternPrefab);

    private void OnReleaseLantern(LanternBehaviour lantern)
    {
        if (lantern.MySpawnPoint != null) lantern.MySpawnPoint.occupiedBy = null;
        lantern.gameObject.SetActive(false);
    }

    private void OnDestroyLantern(LanternBehaviour lantern) => Destroy(lantern.gameObject);

    public void ClearAllLanterns()
    {
        // Return all currently active lanterns to the pool
        foreach (var point in _spawnPoints)
        {
            if (point.occupiedBy != null)
            {
                LanternBehaviour lantern = point.occupiedBy;
                point.occupiedBy = null;
                _lanternPool.Release(lantern);
            }
        }

        // Also search for any "un-pointed" lanterns (like Boost rows) that might be floating
        // This is a bit tricky with ObjectPool unless we track active list.
        // For simplicity, I'll rely on the fact that most will be in spawn points or 
        // I can use FindObjectsOfType if performance allows (small amount of objects).
        LanternBehaviour[] active = FindObjectsOfType<LanternBehaviour>();
        foreach (var l in active)
        {
            if (l.gameObject.activeInHierarchy)
            {
                // We can't easily Release if we don't have the reference, but Shoot() or Despawn() works
                // But let's just use a tag or a list in a real scenario.
                // For this prototype, I'll just deactivate them.
                l.gameObject.SetActive(false);
            }
        }

        Debug.Log("Spawner Cleared.");
    }

    private void SpawnBatch(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            TrySpawnLantern();
        }
    }

    private void HandleLanternEvent(LanternBehaviour lantern)
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsBoostActive) return;
        SpawnBatch(revengeSpawns);
    }
}