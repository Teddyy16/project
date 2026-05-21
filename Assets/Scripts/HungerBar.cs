using UnityEngine;
using UnityEngine.UI;

public class HungerhBar : MonoBehaviour
{
    [Header("UI")]
    public Image fill;
    public Gradient gradient;
    public float changeSpeed = 10f;

    [Header("Values")]
    public float maxValue = 100f;
    public float currentValue = 100f;

    [Header("Save Key")]
    public string saveKey = "Health_Value";

    void Start()
    {
        LoadValue();
        UpdateInstant();
    }

    void Update()
    {
        float target = currentValue / maxValue;

        // Smooth fill animation
        fill.fillAmount = Mathf.Lerp(
            fill.fillAmount,
            target,
            changeSpeed * Time.deltaTime
        );

        // Gradient color
        fill.color = gradient.Evaluate(target);
    }

    // --- PUBLIC ---

    public void SetValue(float amount)
    {
        currentValue = Mathf.Clamp(amount, 0f, maxValue);
        SaveValue();
    }

    public void AddValue(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0f, maxValue);
        SaveValue();
    }

    public void RemoveValue(float amount)
    {
        currentValue = Mathf.Clamp(currentValue - amount, 0f, maxValue);
        SaveValue();
    }

    // --- INTERNAL ---

    void UpdateInstant()
    {
        float ratio = currentValue / maxValue;
        fill.fillAmount = ratio;
        fill.color = gradient.Evaluate(ratio);
    }

    void SaveValue()
    {
        PlayerPrefs.SetFloat(saveKey, currentValue);
        PlayerPrefs.Save();
    }

    void LoadValue()
    {
        if (PlayerPrefs.HasKey(saveKey))
            currentValue = PlayerPrefs.GetFloat(saveKey);
        else
            currentValue = maxValue;
    }
}
