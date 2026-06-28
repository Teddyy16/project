using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public int totalFoodNeeded = 5; 
    public float timeLimit = 60f; 

    public static LevelManager instance;  
    
    public Text timerText;      
    public Text statusText; // Your "Game Over / You Win" text component
    public GameObject clickBlockerPanel; // Your new panel that holds the text

    private int currentFoodCount = 0;
    public bool isGameOver = false;

    void Start()
    {
        // 1. Hide the panel (and its text child) when the game starts
        if (clickBlockerPanel != null) clickBlockerPanel.SetActive(false); 
        
        Time.timeScale = 1f; 
        UpdateTimerUI();
    }


    void Awake()
    {
        // Zorg dat er maar één GameManager bestaat
        if (instance == null) { instance = this; }
    }
    

    void Update()
    {
        if (isGameOver) 
        {
            return;
        }

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
        
        // Set the text first, then turn on the panel
        if (statusText != null) statusText.text = "You Win!";
        if (clickBlockerPanel != null) clickBlockerPanel.SetActive(true);
    }

    void TimeUp()
    {
        isGameOver = true;
        if (timerText != null) timerText.text = "00:00";
        
        // Set the text first, then turn on the panel
        if (statusText != null) statusText.text = "Game Over!";
        if (clickBlockerPanel != null) clickBlockerPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToPlayroom()
    {
        SceneManager.LoadScene("Playroom");
    }
}