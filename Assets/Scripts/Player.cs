using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public HealthBar hungerBar;
    public HealthBar energyBar;

    void Update()
    {
        // --- HUNGER ---
        float newHunger = hungerBar.currentValue - (5f * Time.deltaTime);
        hungerBar.SetValue(newHunger);

        if (Input.GetMouseButtonDown(0))
        {
            hungerBar.SetValue(hungerBar.currentValue - 10f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            hungerBar.SetValue(hungerBar.maxValue);
        }

        // --- ENERGY ---
        float newEnergy = energyBar.currentValue - (10f * Time.deltaTime);
        energyBar.SetValue(newEnergy);

        if (Input.GetMouseButtonDown(2))
        {
            energyBar.SetValue(energyBar.maxValue);
        }

        // --- SCENE SWITCHING ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene("Scene1");

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene("Scene2");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene("Scene3");
    }
}
