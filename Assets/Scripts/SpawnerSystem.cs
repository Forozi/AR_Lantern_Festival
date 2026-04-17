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
        _lanternPool = new ObjectPool<LanternBehaviour>(
            CreateLantern, null, OnReleaseLantern, OnDestroyLantern, true, 12, 20
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

        _waveTimer -= Time.deltaTime;

        // Every 5 seconds, spawn a batch!
        if (_waveTimer <= 0)
        {
            SpawnBatch(lanternsPerWave);
            _waveTimer = waveInterval; // Reset timer for the next 5 seconds
        }
    }

    public void IncreaseCursedChance(float amount)
    {
        cursedLanternChance = Mathf.Clamp01(cursedLanternChance + amount);
    }

    //private void HandleLanternEvent(LanternBehaviour lantern)
    //{
    //    if (GameManager.Instance.IsGameOver) return;
    //    StartCoroutine(SpawnWave());
    //}

    //private IEnumerator SpawnWave()
    //{
    //    TrySpawnLantern();
    //    yield return new WaitForSeconds(secondWaveDelay);
    //    TrySpawnLantern();
    //}

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

    private void SpawnAt(SpawnPoint point)
    {
        bool isCursed = Random.value < cursedLanternChance;
        var type = isCursed ? LanternBehaviour.LanternType.Cursed : LanternBehaviour.LanternType.Blessing;
        var mat = isCursed ? cursedMaterial : blessedMaterial;

        LanternBehaviour lantern = _lanternPool.Get();
        Vector2 offset2D = Random.insideUnitCircle * spawnOffsetRadius;

        lantern.transform.position = point.position + new Vector3(offset2D.x, 0f, offset2D.y);
        point.occupiedBy = lantern;

        // Apply speed multiplier based on stage
        lantern.currentSpeedMultiplier = GetSpeedMultiplierForStage();

        lantern.Initialize(type, mat, _lanternPool, point);
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
        // 1. Lock the spawner
        _isInitialized = false;

        // 2. === CRITICAL FIX === Kill any pending Coroutines (delayed spawns)
        //StopAllCoroutines();

        // 3. Return all currently active lanterns to the pool
        foreach (var point in _spawnPoints)
        {
            if (point.occupiedBy != null)
            {
                LanternBehaviour lantern = point.occupiedBy;
                point.occupiedBy = null; // Free up the spawn point
                _lanternPool.Release(lantern); // Send the lantern away
            }
        }

        // 4. Reset difficulty
        cursedLanternChance = 0.2f;
        Debug.Log("Spawner Reset. All lanterns cleared and coroutines killed.");
    }
    // === NEW METHOD ===
    private void SpawnBatch(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            TrySpawnLantern();
        }
    }

    // === UPDATED EVENT HANDLER ===
    private void HandleLanternEvent(LanternBehaviour lantern)
    {
        if (GameManager.Instance.IsGameOver) return;

        // If a player shoots a lantern, instantly spawn 1 (or more) as "revenge"
        // This keeps the screen busy even between the 5-second waves!
        SpawnBatch(revengeSpawns);
    }

    // You can now DELETE the old "private IEnumerator SpawnWave()" completely!
}