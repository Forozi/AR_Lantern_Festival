using UnityEngine;

public class TapShoot : MonoBehaviour
{
    private Camera _mainCam;

    void Awake()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        // Check for click or screen tap
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            // Increase the max distance if lanterns are far away
            if (Physics.Raycast(ray, out hit, 100f))
            {
                LanternBehaviour lantern = hit.collider.GetComponentInParent<LanternBehaviour>();

                if (lantern != null)
                {
                    lantern.Shoot();
                }
            }
        }
    }
}