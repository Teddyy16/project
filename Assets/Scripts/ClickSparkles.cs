using UnityEngine;

public class ClickSparkles : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparklePrefab;  // Assign your prefab in Inspector
    [SerializeField] private Camera mainCamera;            // Optional: assign Main Camera

    private Camera cam;

    void Start()
    {
        if (mainCamera == null)
            cam = Camera.main;
        else
            cam = mainCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            SpawnSparklesAtMouse();
        }
    }

    void SpawnSparklesAtMouse()
    {
        // Get mouse position in world space (at distance 10 from camera)
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Adjust this distance based on your scene

        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePos);

        // Instantiate and play the particle system
        if (sparklePrefab != null)
        {
            ParticleSystem sparkles = Instantiate(sparklePrefab, worldPosition, Quaternion.identity);
            
            // Optional: Destroy after it finishes playing
            Destroy(sparkles.gameObject, sparkles.main.duration + sparkles.main.startLifetime.constantMax);
        }
    }
}