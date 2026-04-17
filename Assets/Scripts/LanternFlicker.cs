using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LanternFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("How high the emission intensity goes (e.g., 0.5)")]
    [SerializeField] private float maxIntensity = 0.5f;
    [Tooltip("How fast it pulses. Higher = faster.")]
    [SerializeField] private float cycleSpeed = 1.0f;

    private MeshRenderer _renderer;
    private Material _matInstance;
    private Color _baseColor;
    private bool _isSetup = false;

    public void SetupFlicker()
    {
        if (_renderer == null) _renderer = GetComponent<MeshRenderer>();

        // Dummy material
        _matInstance = _renderer.material;

        // Save the original Red or Purple color
        _baseColor = _matInstance.GetColor("_EmissionColor");

        _isSetup = true;
    }

    void Update()
    {
        if (!_isSetup) return;

        // Mathf.PingPong 0 to max max to 0
        float currentIntensity = Mathf.PingPong(Time.time * cycleSpeed, maxIntensity);

        // intensity value
        _matInstance.SetColor("_EmissionColor", _baseColor * currentIntensity);
    }
}