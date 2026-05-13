using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Health
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthBar healthBar;

    // Mana
    public float mana = 100f;
    public float maxMana = 100f;
    public HealthBar manaBar;

    void Update()
    {
        // Health
        healthBar.fillAmount = health / maxHealth;

        health -= 5f * Time.deltaTime;
        health = Mathf.Max(health, 0f);

        if (Input.GetMouseButtonDown(0)) // Left click
            TakeDamage(10f);

        if (Input.GetMouseButtonDown(1)) // Right click
            health = maxHealth;


        // Mana
        manaBar.fillAmount = mana / maxMana;

        mana -= 10f * Time.deltaTime;
        mana = Mathf.Max(mana, 0f);

        if (Input.GetMouseButtonDown(2)) // Middle click
            mana = maxMana;


        // Scene Switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene("Scene1");

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene("Scene2");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene("Scene3");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0f);
    }
}
