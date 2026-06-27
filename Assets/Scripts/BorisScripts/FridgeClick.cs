using UnityEngine;

public class FridgeClick : MonoBehaviour
{
    public FridgeDoorController fridgeDoorController;
    public GlowingEffect glowingEffect;

    private bool fridgeIsOpen = false;

    private void Awake()
    {
        if (glowingEffect == null)
        {
            glowingEffect = GetComponentInParent<GlowingEffect>();
        }
    }

    private void OnMouseDown()
    {
        if (fridgeDoorController == null)
        {
            Debug.LogWarning("FridgeDoorController is not assigned.");
            return;
        }

        fridgeDoorController.ToggleDoor();

        fridgeIsOpen = !fridgeIsOpen;

        if (glowingEffect == null)
        {
            return;
        }

        if (fridgeIsOpen)
        {
            glowingEffect.StopGlow();
        }
        else
        {
            glowingEffect.StartGlow();
        }
    }
}