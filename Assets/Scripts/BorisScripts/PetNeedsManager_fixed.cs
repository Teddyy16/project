using UnityEngine;
using UnityEngine.UI;

public class PetNeedsManager : MonoBehaviour
{
    public static PetNeedsManager Instance;

    [Header("Bars")]
    public Image hungerFill;
    public EnergyBar energyBar;

    [Header("Hunger Settings")]
    public float maxHunger = 100f;
    public float currentHunger = 0f;

    [Tooltip("How much hunger is lost per second after energy becomes full.")]
    public float hungerDecreasePerSecond = 0.7f;

    [Header("Energy Recharge Settings")]
    public float energyRechargeTime = 30f;
    public bool rechargeEnergyWhenHungerIsFull = true;

    [Header("Save")]
    public string hungerSaveKey = "Hunger_Value";

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
            PlayerPrefs.DeleteKey(hungerSaveKey);
            PlayerPrefs.Save();
        }

        currentHunger = PlayerPrefs.GetFloat(hungerSaveKey, 0f);

        if (energyBar == null)
        {
            energyBar = FindObjectOfType<EnergyBar>();
        }

        UpdateHungerBar();

        Debug.Log("PetNeedsManager started. Current hunger: " + currentHunger);
    }

    private void Update()
    {
        if (rechargeEnergyWhenHungerIsFull && IsHungerFull() && energyBar != null && !energyBar.IsFull())
        {
            RechargeEnergy();
        }

        if (energyBar != null && energyBar.IsFull())
        {
            DecreaseHungerOverTime();
        }
    }

    public void AddHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        SaveHunger();
        UpdateHungerBar();

        Debug.Log("Hunger increased by " + amount + ". Current hunger: " + currentHunger);
    }

    private void DecreaseHungerOverTime()
    {
        if (currentHunger <= 0f)
        {
            currentHunger = 0f;
            UpdateHungerBar();
            return;
        }

        currentHunger -= hungerDecreasePerSecond * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        SaveHunger();
        UpdateHungerBar();
    }

    public bool IsHungerFull()
    {
        return currentHunger >= maxHunger;
    }

    private void RechargeEnergy()
    {
        if (energyBar == null)
        {
            return;
        }

        if (energyBar.IsFull())
        {
            return;
        }

        float energyPerSecond = energyBar.maxValue / energyRechargeTime;
        float energyToAdd = energyPerSecond * Time.deltaTime;

        energyBar.AddEnergy(energyToAdd);
    }

    private void UpdateHungerBar()
    {
        if (hungerFill == null)
        {
            Debug.LogWarning("Hunger Fill is missing in PetNeedsManager.");
            return;
        }

        float ratio = currentHunger / maxHunger;

        hungerFill.type = Image.Type.Filled;
        hungerFill.fillMethod = Image.FillMethod.Horizontal;
        hungerFill.fillOrigin = 0;
        hungerFill.fillAmount = ratio;
    }

    public void SetHunger(float value)
    {
        currentHunger = Mathf.Clamp(value, 0f, maxHunger);

        SaveHunger();
        UpdateHungerBar();
    }

    public void ClearHunger()
    {
        SetHunger(0f);
    }

    private void SaveHunger()
    {
        PlayerPrefs.SetFloat(hungerSaveKey, currentHunger);
        PlayerPrefs.Save();
    }

    private void OnDisable()
    {
        SaveHunger();
    }
}