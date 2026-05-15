using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    // Static instance allows SceneManagerScript to find this specific room's canvas energy bar
    public static EnergyManager Instance;

    [Header("UI Elements (Drag your Fill Image here)")]
    public Image energyFillImage;
    public Gradient energyGradient;

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;

    // Key tag used to save/load from the mobile device's storage
    private const string EnergySaveKey = "PlayerCurrentEnergy";

    void Awake()
    {
        // Links this active scene's canvas to the static instance
        Instance = this;
    }

    void Start()
    {
        // Loads the saved value right as the room opens, then updates the bar 
        LoadEnergy();
        UpdateEnergyBar();
    }

    
    /// Deducts energy, makes sure its above 0, updates the visuals, and saves to the local storage of the specific device
    
    public void DrainEnergy(float amount)
    {
        currentEnergy -= amount;
        
        // Safety check to ensure energy never goes into negative numbers
        if (currentEnergy < 0) 
        {
            currentEnergy = 0;
        }

        UpdateEnergyBar();
        SaveEnergy(); 
    }

    
    /// Puts the raw numbers into UI fill percentages and gradients
    
    public void UpdateEnergyBar()
    {
        if (energyFillImage == null) return;

        // Calculates decimal ratio 
        float currentRatio = currentEnergy / maxEnergy;
        
        // Updates the horizontal slice percentage
        energyFillImage.fillAmount = currentRatio;

        // Pulls the needed tint color from the custom gradient 
        energyFillImage.color = energyGradient.Evaluate(currentRatio);
    }

    

    void SaveEnergy()
    {
        PlayerPrefs.SetFloat(EnergySaveKey, currentEnergy);
        PlayerPrefs.Save(); // Writes the file to the phone's storage
    }

    void LoadEnergy()
    {
        // If the player has a save history, read it. Otherwise, give them full energy.
        if (PlayerPrefs.HasKey(EnergySaveKey))
        {
            currentEnergy = PlayerPrefs.GetFloat(EnergySaveKey);
        }
        else
        {
            currentEnergy = maxEnergy;
        }
    }

    // Safeguard: Automatically saves if a mobile player minimizes or backgrounds the app
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveEnergy();
        }
    }
}

