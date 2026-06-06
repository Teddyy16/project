using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private float zDepth;

    void Start()
    {
       
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // Calculate how far away the object is from the camera when clicked
        zDepth = mainCamera.WorldToScreenPoint(transform.position).z;

        // Temporarily disable gravity and freeze rotations so it doesn't spin wildly
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnMouseDrag()
    {
        // Get the mouse position in world space using our locked z-depth
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // Calculate the direction to the mouse and move via velocity
        // The '15f' is a speed multiplier, can increase it to make it faster
        Vector3 moveDirection = targetWorldPos - transform.position;
        rb.linearVelocity = moveDirection * 12f; 
    }

    void OnMouseUp()
    {
        // Drops and returns gravity back on
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }
}
