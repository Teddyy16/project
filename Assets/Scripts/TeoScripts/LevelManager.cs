using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int totalFoodNeeded = 20; // Total food objects in the level
    public float timeLimit = 60f;   // Time limit in seconds
    
    public Text timerText;      // Drag your Timer Text here
    public GameObject winScreen;    // Drag your WinScreen Panel here

    private int currentFoodCount = 0;
    private bool isGameOver = false;

    void Start()
    {
        winScreen.SetActive(false); // Make sure win screen is hidden
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // 1. Countdown the timer
        if (timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            TimeUp();
        }
    }

    // 2. This is called by containers whenever a food item snaps inside
    public void AddFood()
    {
        if (isGameOver) return;

        currentFoodCount++;
        Debug.Log("Food Added! Count: " + currentFoodCount + "/" + totalFoodNeeded);

        if (currentFoodCount >= totalFoodNeeded)
        {
            WinGame();
        }
    }

    void UpdateTimerUI()
    {
        // Format time cleanly into Minutes:Seconds
        int minutes = Mathf.FloorToInt(timeLimit / 60);
        int seconds = Mathf.FloorToInt(timeLimit % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void WinGame()
    {
        isGameOver = true;
        winScreen.SetActive(true); // Show the "You Win!" screen
        Time.timeScale = 0f;       // Optional: Freeze the game physics
    }

    void TimeUp()
    {
        isGameOver = true;
        timerText.text = "00:00";
        Debug.Log("Game Over! You ran out of time.");
        // Optional: Show a Lose Screen panel here
    }
}