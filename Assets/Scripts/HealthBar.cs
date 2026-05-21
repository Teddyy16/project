using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fill;
    public Gradient gradient;
    public float changeSpeed = 10f;

    public float maxValue = 100f;
    public float currentValue = 100f;

    public string saveKey = "Stat_Value";

    void Start()
    {
        currentValue = PlayerPrefs.GetFloat(saveKey, maxValue);

        float ratio = currentValue / maxValue;
        fill.fillAmount = ratio;
        fill.color = gradient.Evaluate(ratio);
    }

    void Update()
    {
        float target = currentValue / maxValue;

        fill.fillAmount = Mathf.Lerp(fill.fillAmount, target, changeSpeed * Time.deltaTime);
        fill.color = gradient.Evaluate(target);
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);
        PlayerPrefs.SetFloat(saveKey, currentValue);
    }
}
