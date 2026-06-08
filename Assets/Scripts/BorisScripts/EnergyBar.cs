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
    public float currentValue = 0f;

    [Header("Save")]
    public string saveKey = "Energy_Value";

    [Header("Testing")]
    public bool resetSavedValueOnStart = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (resetSavedValueOnStart)
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
        }

        currentValue = PlayerPrefs.GetFloat(saveKey, 0f);
        UpdateBarInstant();

        Debug.Log("EnergyBar started. Current energy: " + currentValue);
    }

    private void Update()
    {
        if (fill == null)
        {
            return;
        }

        float target = currentValue / maxValue;

        fill.fillAmount = Mathf.Lerp(fill.fillAmount, target, changeSpeed * Time.deltaTime);

        if (gradient != null)
        {
            fill.color = gradient.Evaluate(target);
        }
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

    public void ClearEnergy()
    {
        SetValue(0f);
    }

    private void UpdateBarInstant()
    {
        if (fill == null)
        {
            return;
        }

        float ratio = currentValue / maxValue;

        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = ratio;

        if (gradient != null)
        {
            fill.color = gradient.Evaluate(ratio);
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(saveKey, currentValue);
        PlayerPrefs.Save();
    }
}