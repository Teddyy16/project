using UnityEngine;
using UnityEngine.UI;

public class PetNeedsManager : MonoBehaviour
{
    public static PetNeedsManager Instance;

    [Header("UI Fill Images")]
    public Image hungerFill;
    public Image energyFill;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float currentHunger = 0f;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy = 0f;
    public float energyRechargeTime = 30f;

    [Header("Settings")]
    public bool rechargeEnergyOnlyWhenHungerIsFull = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateBars();
    }

    private void Update()
    {
        if (rechargeEnergyOnlyWhenHungerIsFull && currentHunger >= maxHunger)
        {
            RechargeEnergyOverTime();
        }
    }

    public void AddHunger(float amount)
    {
        currentHunger += amount;

        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        Debug.Log("Hunger increased by " + amount + ". Current hunger: " + currentHunger);

        UpdateBars();
    }

    private void RechargeEnergyOverTime()
    {
        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
            UpdateBars();
            return;
        }

        float energyPerSecond = maxEnergy / energyRechargeTime;
        currentEnergy += energyPerSecond * Time.deltaTime;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        UpdateBars();
    }

    public void AddEnergy(float amount)
    {
        currentEnergy += amount;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        UpdateBars();
    }

    public void SetHunger(float value)
    {
        currentHunger = Mathf.Clamp(value, 0f, maxHunger);
        UpdateBars();
    }

    public void SetEnergy(float value)
    {
        currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (hungerFill != null)
        {
            hungerFill.fillAmount = currentHunger / maxHunger;
        }

        if (energyFill != null)
        {
            energyFill.fillAmount = currentEnergy / maxEnergy;
        }
    }
}
