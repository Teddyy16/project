using UnityEngine;

public class ContainerSnapZone : MonoBehaviour
{
    public float forwardZ;         // The Z-depth of your baskets/food track
    public float slideSpeed = 5f;  // How fast the container glides (higher = faster)
    
    private float originalZ;       // The starting Z-depth inside the fridge
    private float targetZ;         // The Z position the container is currently moving towards
    private bool isPulledForward = false;

    void Start()
    {
        // Remember where it started inside the fridge using localPosition
        originalZ = transform.localPosition.z;
        targetZ = originalZ; // Start at rest
    }

    void OnMouseDown()
    {
        // Toggle the target Z position when clicked
        if (!isPulledForward)
        {
            targetZ = forwardZ;
            isPulledForward = true;
            Debug.Log("Gliding forward...");
        }
        else
        {
            targetZ = originalZ;
            isPulledForward = false;
            Debug.Log("Gliding back...");
        }
    }

    void Update()
    {
        // 1. Get our current local position
        Vector3 currentPos = transform.localPosition;

        // 2. Create the exact target position vector using our current X and Y
        Vector3 targetPos = new Vector3(currentPos.x, currentPos.y, targetZ);

        // 3. Smoothly interpolate from our current position to the target position over time
        transform.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * slideSpeed);
    }

   private void OnTriggerEnter(Collider other)
    {
        if (isPulledForward && other.CompareTag("Food"))
    {
        // Get the drag script attached to the food piece
        DragAndDrop foodScript = other.GetComponent<DragAndDrop>();

        // ONLY count it if it hasn't been counted yet!
        if (foodScript != null && !foodScript.hasBeenCounted)
        {
            foodScript.hasBeenCounted = true; // Mark it as counted immediately!

            other.transform.position = transform.position;

            Rigidbody foodRb = other.GetComponent<Rigidbody>();
            if (foodRb != null)
            {
                foodRb.isKinematic = true;
                foodRb.linearVelocity = Vector3.zero;
            }

            other.transform.SetParent(this.transform);
            
            // Alert the Level Manager
            LevelManager manager = FindFirstObjectByType<LevelManager>();
            if (manager != null)
            {
                manager.AddFood();
            }

            Debug.Log(other.gameObject.name + " snapped and successfully counted once.");
        }
    }
}
}