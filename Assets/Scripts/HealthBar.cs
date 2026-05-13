using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fill;
    public float changeSpeed = 10.0f;

    public float fillAmount { get; set; } = 1.0f;

    void Update()
    {
        fill.fillAmount = Mathf.Lerp(
            fill.fillAmount,
            fillAmount,
            changeSpeed * Time.deltaTime);
    }
}