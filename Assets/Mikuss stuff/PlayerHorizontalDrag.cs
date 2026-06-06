using UnityEngine;

public class PlayerHorizontalDrag : MonoBehaviour
{
    public float minX = -2.5f;
    public float maxX = 2.5f;

    private float fixedY;
    private float fixedZ;
    private float screenZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
        screenZ = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = screenZ;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        float clampedX = Mathf.Clamp(worldPosition.x, minX, maxX);

        transform.position = new Vector3(clampedX, fixedY, fixedZ);
    }
}