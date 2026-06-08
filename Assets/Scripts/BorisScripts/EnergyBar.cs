using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public static EnergyBar Instance;

    [Header("UI")]
    public Image fill;
    public Gradient gradient;
    public float changeSpeed = 10f;

    [Header("Energy Settings")]
    public float maxValue = 100f;
    public float currentValue = 100f;

    [Header("Save")]
    public string saveKey = "Energy_Value";

    void Awake()
    {
        Instance = this;
    }

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

    public void AddEnergy(float amount)
    {
        ChangeValue(amount);
    }

    public bool TryUseEnergy(float amount)
    {
        if (currentValue < amount)
        {
            Debug.Log("Not enough energy!");
            return false;
        }

        ChangeValue(-amount);
        return true;
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentValue >= amount;
    }

    public bool IsFull()
    {
        return currentValue >= maxValue;
    }

    public float GetPercent()
    {
        return currentValue / maxValue;
    }

    public void SetToFull()
    {
        SetValue(maxValue);
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