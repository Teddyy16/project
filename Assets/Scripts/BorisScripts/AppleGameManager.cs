using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AppleGameManager : MonoBehaviour
{
    public static AppleGameManager Instance;

    [Header("Game Settings")]
    public float gameTime = 30f;
    public int applesNeededToUnlock = 20;

    [Header("Scene")]
    public string playroomSceneName = "Playroom";

    [Header("Unlock Save")]
    public string unlockSaveKey = "Unlocked_Apple_Item";

    [Header("UI")]
    public Text timerText;
    public Text appleCounterText;
    public Text gameOverText;
    public Text unlockText;
    public Button backButton;

    [Header("UI Settings")]
    public int uiFontSize = 42;
    public int messageFontSize = 60;

    private float currentTime;
    private int appleCount = 0;
    private bool gameRunning = true;

    private Font defaultFont;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CreateUIIfMissing();

        currentTime = gameTime;
        appleCount = 0;
        gameRunning = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (unlockText != null)
        {
            unlockText.gameObject.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoBackToPlayroom);
        }

        UpdateTimerUI();
        UpdateAppleCounterUI();
    }

    private void Update()
    {
        if (!gameRunning)
        {
            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }

    public void AddApple()
    {
        if (!gameRunning)
        {
            return;
        }

        appleCount++;
        UpdateAppleCounterUI();

        Debug.Log("Apple collected. Current apples: " + appleCount);
    }

    public void HitFork()
    {
        if (!gameRunning)
        {
            return;
        }

        gameRunning = false;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Game Over!";
        }

        CheckUnlockReward();

        Debug.Log("Player hit a fork. Game Over.");
    }

    public void GameOver()
    {
        HitFork();
    }

    private void EndGame()
    {
        if (!gameRunning)
        {
            return;
        }

        gameRunning = false;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Time's up!";
        }

        CheckUnlockReward();

        Debug.Log("Apple mini-game ended. Apples collected: " + appleCount);
    }

    private void CheckUnlockReward()
    {
        if (appleCount >= applesNeededToUnlock)
        {
            PlayerPrefs.SetInt(unlockSaveKey, 1);
            PlayerPrefs.Save();

            if (unlockText != null)
            {
                unlockText.gameObject.SetActive(true);
                unlockText.text = "You unlocked item!";
            }

            Debug.Log("Unlocked item from Apple mini-game.");
        }
        else
        {
            if (unlockText != null)
            {
                unlockText.gameObject.SetActive(false);
            }

            Debug.Log("Not enough apples to unlock item. Needed: " + applesNeededToUnlock + ", collected: " + appleCount);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
        }
    }

    private void UpdateAppleCounterUI()
    {
        if (appleCounterText != null)
        {
            appleCounterText.text = "Apples: " + appleCount.ToString();
        }
    }

    private void CreateUIIfMissing()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (timerText == null)
        {
            timerText = CreateText("TimerText", canvas.transform, "Time: 30", uiFontSize);

            RectTransform rect = timerText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -40f);
            rect.sizeDelta = new Vector2(500f, 80f);
        }

        if (appleCounterText == null)
        {
            appleCounterText = CreateText("AppleCounterText", canvas.transform, "Apples: 0", uiFontSize);

            RectTransform rect = appleCounterText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -110f);
            rect.sizeDelta = new Vector2(500f, 80f);
        }

        if (gameOverText == null)
        {
            gameOverText = CreateText("GameOverText", canvas.transform, "Game Over!", messageFontSize);

            RectTransform rect = gameOverText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 80f);
            rect.sizeDelta = new Vector2(900f, 120f);
        }

        if (unlockText == null)
        {
            unlockText = CreateText("UnlockText", canvas.transform, "You unlocked item!", messageFontSize);

            RectTransform rect = unlockText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -20f);
            rect.sizeDelta = new Vector2(900f, 120f);
        }

        if (backButton == null)
        {
            backButton = CreateButton("BackButton", canvas.transform, "Back", 34);

            RectTransform rect = backButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-40f, -40f);
            rect.sizeDelta = new Vector2(220f, 80f);
        }
    }

    private Text CreateText(string objectName, Transform parent, string text, int fontSize)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();

        Text uiText = textObject.AddComponent<Text>();
        uiText.font = defaultFont;
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;

        Outline outline = textObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);

        return uiText;
    }

    private Button CreateButton(string objectName, Transform parent, string text, int textSize)
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.95f, 0.35f, 0.65f, 1f);

        Button button = buttonObject.AddComponent<Button>();

        Text buttonText = CreateText("Text", buttonObject.transform, text, textSize);

        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;

        return button;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoBackToPlayroom()
    {
        SceneManager.LoadScene(playroomSceneName);
    }
}