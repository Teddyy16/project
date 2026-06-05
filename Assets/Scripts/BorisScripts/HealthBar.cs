using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Image fill;
    public Gradient gradient;
    public float changeSpeed = 10f;

    [Header("Bar Settings")]
    public float maxValue = 100f;
    public float currentValue = 100f;

    [Header("Save")]
    public string saveKey = "Stat_Value";

    void Start()
    {
        currentValue = PlayerPrefs.GetFloat(saveKey, maxValue);
        UpdateBarInstant();
    }

    void Update()
    {
        if (fill == null) return;

        float target = currentValue / maxValue;

        fill.fillAmount = Mathf.Lerp(fill.fillAmount, target, changeSpeed * Time.deltaTime);
        fill.color = gradient.Evaluate(target);
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);

        PlayerPrefs.SetFloat(saveKey, currentValue);
        PlayerPrefs.Save();

        UpdateBarInstant();
    }

    public void ChangeValue(float amount)
    {
        SetValue(currentValue + amount);
    }

    private void UpdateBarInstant()
    {
        if (fill == null) return;

        float ratio = currentValue / maxValue;

        fill.fillAmount = ratio;
        fill.color = gradient.Evaluate(ratio);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(saveKey, currentValue);
        PlayerPrefs.Save();
    }
}
