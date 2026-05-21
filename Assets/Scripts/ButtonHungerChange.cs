using UnityEngine;

public class ButtonHungerChange : MonoBehaviour
{
    public HealthBar hungerBar;

    public void ChangeHunger(float amount)
    {
        hungerBar.ChangeValue(amount);
    }
}