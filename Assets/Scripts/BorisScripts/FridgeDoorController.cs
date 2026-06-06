using UnityEngine;

public class FridgeDoorController : MonoBehaviour
{
    [Header("Door")]
    public Transform doorPivot;

    [Header("Angles")]
    public float closedAngle = 0f;
    public float openAngle = 90f;

    [Header("Speed")]
    public float speed = 4f;

    private bool isOpen = false;

    void Update()
    {
        if (doorPivot == null)
        {
            return;
        }

        float targetAngle = isOpen ? openAngle : closedAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        doorPivot.localRotation = Quaternion.Lerp(
            doorPivot.localRotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}
