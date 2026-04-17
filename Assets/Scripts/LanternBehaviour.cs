using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class LanternBehaviour : MonoBehaviour
{
    public enum LanternType { Blessing, Cursed, Boost }
    public LanternType type;

    [Header("Settings")]
    [SerializeField] private float floatSpeed = 0.5f;
    [SerializeField] private float maxHeight = 8.0f;
    [SerializeField] private float swayRadius = 0.3f; // How far it drifts side-to-side
    [SerializeField] private float swaySpeed = 1.0f;  // How fast it drifts
    public float currentSpeedMultiplier = 1.0f;

    [Header("References")]
    [Tooltip("Drag the 'Lantern Object' here so we can hide all meshes at once")]
    public GameObject lanternVisuals;
    [Tooltip("Drag the 'MainBody' here so we can change its color (Blessed/Cursed)")]
    public MeshRenderer mainBodyRenderer;
    public Collider lanternCollider;

    [Header("Effects")]
    public ParticleSystem smallPopEffect;
    public ParticleSystem bigBurstEffect;

    public static event System.Action<LanternBehaviour> OnLanternHit;
    public static event System.Action<LanternBehaviour> OnLanternEscaped;

    private IObjectPool<LanternBehaviour> _pool;
    public SpawnerSystem.SpawnPoint MySpawnPoint { get; private set; }

    private bool _isActive = false;
    private Vector3 _basePosition; // Tracks the straight upward path
    private float _randomTimeOffset; // Prevents all lanterns from swaying in sync

    public void Initialize(LanternType type, Material mat, IObjectPool<LanternBehaviour> pool, SpawnerSystem.SpawnPoint point)
    {
        this.type = type;
        this._pool = pool;
        this.MySpawnPoint = point;

        // === UPDATED SECTION ===
        if (mainBodyRenderer != null)
        {
            mainBodyRenderer.material = mat; // Set Red or Purple

            // Tell the flicker script to grab the new color and start looping
            LanternFlicker flicker = mainBodyRenderer.GetComponent<LanternFlicker>();
            if (flicker != null) flicker.SetupFlicker();
        }
        // =======================

        _randomTimeOffset = Random.Range(0f, 100f);
        _basePosition = transform.position;

        _isActive = true;
        lanternVisuals.SetActive(true);
        lanternCollider.enabled = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_isActive) return;

        // 1. Move the base position straight up
        _basePosition += Vector3.up * floatSpeed * currentSpeedMultiplier * Time.deltaTime;

        // 2. Calculate a smooth circular drift using Sine and Cosine
        float offsetX = Mathf.Sin((Time.time + _randomTimeOffset) * swaySpeed) * swayRadius;
        float offsetZ = Mathf.Cos((Time.time + _randomTimeOffset) * swaySpeed) * swayRadius;

        // 3. Apply the combined position
        transform.position = _basePosition + new Vector3(offsetX, 0f, offsetZ);

        if (transform.position.y > maxHeight)
        {
            OnLanternEscaped?.Invoke(this);
            StartCoroutine(PopSequence(true)); // Escaped = Big Burst
        }
    }

    public void Shoot()
    {
        if (!_isActive) return;
        OnLanternHit?.Invoke(this);
        StartCoroutine(PopSequence(false)); // Hit = Small Pop
    }

    private IEnumerator PopSequence(bool wasEscaped)
    {
        _isActive = false;

        // Hide visuals and disable collision immediately
        lanternVisuals.SetActive(false);
        lanternCollider.enabled = false;

        ParticleSystem activeEffect = wasEscaped ? bigBurstEffect : smallPopEffect;

        if (activeEffect != null)
        {
            // If it's a small pop, set the color to match the lantern type
            if (!wasEscaped)
            {
                var main = activeEffect.main;
                main.startColor = new ParticleSystem.MinMaxGradient(type == LanternType.Cursed ? Color.magenta : Color.red);
            }

            activeEffect.Play();
            yield return new WaitForSeconds(activeEffect.main.duration);
        }

        Despawn();
    }

    private void Despawn()
    {
        if (MySpawnPoint != null) MySpawnPoint.occupiedBy = null;
        _pool?.Release(this);
    }
}