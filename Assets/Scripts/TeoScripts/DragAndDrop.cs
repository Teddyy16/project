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
        // Calculate how far away the object is from the camera on click to lock its track
        zDepth = mainCamera.WorldToScreenPoint(transform.position).z;

        // Turn off gravity so it doesn't fall while you hold it
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnMouseDrag()
    {
        // 1. Get the current mouse position in pixels
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        
        // 2. Convert it into the 3D world coordinates
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // 3. Move the object directly to the mouse position
        transform.position = targetWorldPos;
    }

    void OnMouseUp()
    {
        // Let go and turn gravity back on so it falls naturally
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }
}