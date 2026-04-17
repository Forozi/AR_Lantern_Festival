using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARPlacementManager : MonoBehaviour
{
    [Header("AR References")]
    private ARRaycastManager _raycastManager;
    private ARPlaneManager _planeManager;
    private Camera _arCamera;

    [Header("System References")]
    [SerializeField] private SpawnerSystem spawnerSystem;

    [Header("Visuals & UI")]
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject startButtonPanel;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private bool _isPlaced = false;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        _arCamera = Camera.main;

        placementIndicator.SetActive(false);
        if (startButtonPanel != null) startButtonPanel.SetActive(false);
    }

    void Update()
    {
        if (_isPlaced) return;

        Ray ray = _arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (_raycastManager.Raycast(ray, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (startButtonPanel != null) startButtonPanel.SetActive(true);
        }
        else
        {
            placementIndicator.SetActive(false);
            if (startButtonPanel != null) startButtonPanel.SetActive(false);
        }
    }

    public void ConfirmPlacementAndStart()
    {
        _isPlaced = true;

        placementIndicator.SetActive(false);
        if (startButtonPanel != null) startButtonPanel.SetActive(false);

        // Disable Plane Detection to save battery/CPU
        SetARPlanesActive(false);

        spawnerSystem.SetAnchorAndInitialize(placementIndicator.transform);
        GameManager.Instance.StartGame();
    }

    // Restart Game
    public void ResetPlacement()
    {
        _isPlaced = false; // reset placement

        // Enable Plane Detection
        SetARPlanesActive(true);

        Debug.Log("AR Placement Reset. Scanning for planes again.");
    }

    private void SetARPlanesActive(bool isActive)
    {
        if (_planeManager == null) return;

        // Stop/Start the detection engine
        _planeManager.enabled = isActive;

        // Hide/Show plane symbol
        foreach (var plane in _planeManager.trackables)
        {
            plane.gameObject.SetActive(isActive);
        }
    }
}