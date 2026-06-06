using UnityEngine;

public class FridgeClick : MonoBehaviour
{
    public FridgeDoorController fridgeDoorController;

    private void OnMouseDown()
    {
        if (fridgeDoorController != null)
        {
            fridgeDoorController.ToggleDoor();
        }
        else
        {
            Debug.LogWarning("FridgeDoorController is not assigned.");
        }
    }
}
