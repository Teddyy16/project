using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private const string HungerKey = "PlayerHunger";
    private const string EnergyKey = "PlayerEnergy";

    // Hunger
    public float hunger = 100f;
    public float maxHunger = 100f;
    public HealthBar hungerBar;

    // Energy
    public float energy = 100f;
    public float maxEnergy = 100f;
    public HealthBar energyBar;

    void Start()
    {
        hunger = PlayerPrefs.GetFloat(HungerKey, maxHunger);
        energy = PlayerPrefs.GetFloat(EnergyKey, maxEnergy);

        hungerBar.SetValue(hunger);
        energyBar.SetValue(energy);
    }

    void Update()
    {
        // --- HUNGER ---
        hunger -= 5f * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        hungerBar.SetValue(hunger);
        PlayerPrefs.SetFloat(HungerKey, hunger);

        if (Input.GetMouseButtonDown(0))
        {
            hunger -= 10f;
            hunger = Mathf.Clamp(hunger, 0f, maxHunger);
            hungerBar.SetValue(hunger);
            PlayerPrefs.SetFloat(HungerKey, hunger);
        }

        if (Input.GetMouseButtonDown(1))
        {
            hunger = maxHunger;
            hungerBar.SetValue(hunger);
            PlayerPrefs.SetFloat(HungerKey, hunger);
        }

        // --- ENERGY ---
        energy -= 10f * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0f, maxEnergy);
        energyBar.SetValue(energy);
        PlayerPrefs.SetFloat(EnergyKey, energy);

        if (Input.GetMouseButtonDown(2))
        {
            energy = maxEnergy;
            energyBar.SetValue(energy);
            PlayerPrefs.SetFloat(EnergyKey, energy);
        }

        // --- SCENE SWITCHING ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene("Scene1");

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene("Scene2");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene("Scene3");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PlayerPrefs.SetFloat(HungerKey, hunger);
            PlayerPrefs.SetFloat(EnergyKey, energy);
            PlayerPrefs.Save();
        }
    }
}
