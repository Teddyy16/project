using UnityEngine;
using UnityEngine.UI;

public class AppleGameManager : MonoBehaviour
{
    public static AppleGameManager Instance;

    [Header("Game")]
    public float gameTime = 30f;
    public GameObject spawnerObject;

    private float currentTime;
    private int applesCaught;
    private bool gameRunning;

    private Text timerText;
    private Text scoreText;
    private Text endText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = gameTime;
        applesCaught = 0;
        gameRunning = true;

        CreateUI();
        UpdateUI();
    }

    void Update()
    {
        if (!gameRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TimeIsUp();
        }

        UpdateUI();
    }

    public void AddApple()
    {
        if (!gameRunning) return;

        applesCaught++;
        UpdateUI();
    }

    public void GameOver()
    {
        if (!gameRunning) return;

        gameRunning = false;

        if (spawnerObject != null)
        {
            spawnerObject.SetActive(false);
        }

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            endText.text = "GAME OVER!\nApples caught: " + applesCaught;
        }

        Debug.Log("Game over. Apples caught: " + applesCaught);
    }

    private void TimeIsUp()
    {
        gameRunning = false;

        if (spawnerObject != null)
        {
            spawnerObject.SetActive(false);
        }

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            endText.text = "Time is up!\nApples caught: " + applesCaught;
        }

        Debug.Log("Time is up. Apples caught: " + applesCaught);
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime);
        }

        if (scoreText != null)
        {
            scoreText.text = "Apples: " + applesCaught;
        }
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject("AppleGameUICanvas");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        timerText = CreateText("TimerText", canvas.transform, "Time: 30", new Vector2(0, -90), 70);
        scoreText = CreateText("ScoreText", canvas.transform, "Apples: 0", new Vector2(0, -180), 70);

        endText = CreateText("EndText", canvas.transform, "", new Vector2(0, -520), 90);
        endText.gameObject.SetActive(false);
    }

    private Text CreateText(string objectName, Transform parent, string text, Vector2 position, int fontSize)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(1000, 160);

        Text uiText = obj.AddComponent<Text>();
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.black;
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return uiText;
    }
}