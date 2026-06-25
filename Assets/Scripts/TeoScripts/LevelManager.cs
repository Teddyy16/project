using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // NEW: Required for reloading the level!

public class LevelManager : MonoBehaviour
{
    public int totalFoodNeeded = 5; 
    public float timeLimit = 60f;   
    
    public Text timerText;      
    public GameObject winScreen;    
    public GameObject loseScreen;   // NEW: Slot for your Lose Screen panel

    private int currentFoodCount = 0;
    private bool isGameOver = false;

    void Start()
    {
        // Make sure both screens are hidden when the game starts
        if (winScreen != null) winScreen.SetActive(false); 
        if (loseScreen != null) loseScreen.SetActive(false); 
        
        Time.timeScale = 1f; // Reset time speed back to normal
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver) return;

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

    public void AddFood()
    {
        if (isGameOver) return;

        currentFoodCount++;

        if (currentFoodCount >= totalFoodNeeded)
        {
            WinGame();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeLimit / 60);
        int seconds = Mathf.FloorToInt(timeLimit % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void WinGame()
    {
        isGameOver = true;
        if (winScreen != null) winScreen.SetActive(true); 
        Time.timeScale = 0f; // Freeze gameplay       
    }

    void TimeUp()
    {
        isGameOver = true;
        if (timerText != null) timerText.text = "00:00";
        
        // NEW: Show the Lose Screen instead of just printing a log
        if (loseScreen != null) 
        {
            loseScreen.SetActive(true);
        }
        
        Time.timeScale = 0f; // Freeze gameplay so they can't keep dragging food
    }

    // NEW: Call this function when the Try Again button is pressed
    public void RestartLevel()
    {
        // Reloads whatever level is currently active
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public void GoToPlayroom()
    {
        // Make sure time isn't frozen when returning to the playroom
        Time.timeScale = 1f; 
        
        // Load your main menu/playroom scene
        // REPLACE "PlayroomScene" with the EXACT name of your playroom scene file!
        SceneManager.LoadScene("Playroom");
    }
}