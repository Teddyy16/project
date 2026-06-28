using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Font")]
    public TMP_FontAsset gameFont;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI appleCounterText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI unlockText;
    public Button backButton;

    [Header("Text Sizes")]
    public float uiFontSize = 260f;
    public float uiTextScale = 1.45f;

    public float messageFontSize = 170f;
    public float unlockFontSize = 88f;
    public float buttonFontSize = 60f;

    [Header("3D Text Effect")]
    public Color textFaceColor = Color.white;

    public Color textDepthColor =
        new Color(0.22f, 0.42f, 0.9f, 1f);

    public Color textRimColor =
        new Color(0.8f, 0.92f, 1f, 1f);

    [Range(0f, 0.5f)]
    public float textDepthSize = 0.16f;

    [Range(-1f, 1f)]
    public float textDepthOffsetX = 0.015f;

    [Range(-1f, 1f)]
    public float textDepthOffsetY = -0.18f;

    private float currentTime;
    private int appleCount;
    private bool gameRunning = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateUIIfMissing();

        currentTime = gameTime;
        appleCount = 0;
        gameRunning = true;

        ApplyStyleToAllTexts();
        SetAllTextPositions();

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
        }
        else
        {
            if (unlockText != null)
            {
                unlockText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime);
        }
    }

    private void UpdateAppleCounterUI()
    {
        if (appleCounterText != null)
        {
            appleCounterText.text = "Apples: " + appleCount;
        }
    }

    private void ApplyStyleToAllTexts()
    {
        ApplyAxolotl3DStyle(
            timerText,
            uiFontSize,
            uiTextScale
        );

        ApplyAxolotl3DStyle(
            appleCounterText,
            uiFontSize,
            uiTextScale
        );

        ApplyAxolotl3DStyle(
            gameOverText,
            messageFontSize,
            1f
        );

        ApplyAxolotl3DStyle(
            unlockText,
            unlockFontSize,
            1f
        );

        if (backButton != null)
        {
            TextMeshProUGUI buttonText =
                backButton.GetComponentInChildren<TextMeshProUGUI>();

            ApplyAxolotl3DStyle(
                buttonText,
                buttonFontSize,
                1f
            );
        }
    }

    private void ApplyAxolotl3DStyle(
        TextMeshProUGUI text,
        float fontSize,
        float textScale
    )
    {
        if (text == null)
        {
            return;
        }

        if (gameFont != null)
        {
            text.font = gameFont;
        }

        text.enableAutoSizing = false;
        text.fontSize = fontSize;
        text.fontSizeMin = fontSize;
        text.fontSizeMax = fontSize;

        text.color = textFaceColor;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;

        text.rectTransform.localScale =
            Vector3.one * textScale;

        Material styleMaterial =
            new Material(text.fontSharedMaterial);

        styleMaterial.EnableKeyword("UNDERLAY_ON");

        styleMaterial.SetColor(
            ShaderUtilities.ID_FaceColor,
            textFaceColor
        );

        styleMaterial.SetFloat(
            ShaderUtilities.ID_OutlineWidth,
            0.035f
        );

        styleMaterial.SetColor(
            ShaderUtilities.ID_OutlineColor,
            textRimColor
        );

        styleMaterial.SetColor(
            ShaderUtilities.ID_UnderlayColor,
            textDepthColor
        );

        styleMaterial.SetFloat(
            ShaderUtilities.ID_UnderlayOffsetX,
            textDepthOffsetX
        );

        styleMaterial.SetFloat(
            ShaderUtilities.ID_UnderlayOffsetY,
            textDepthOffsetY
        );

        styleMaterial.SetFloat(
            ShaderUtilities.ID_UnderlayDilate,
            textDepthSize
        );

        styleMaterial.SetFloat(
            ShaderUtilities.ID_UnderlaySoftness,
            0f
        );

        text.fontMaterial = styleMaterial;
        text.UpdateMeshPadding();
    }

    private void SetAllTextPositions()
    {
        if (timerText != null)
        {
            RectTransform rect = timerText.rectTransform;

            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            rect.anchoredPosition = new Vector2(0f, -35f);
            rect.sizeDelta = new Vector2(1400f, 340f);
        }

        if (appleCounterText != null)
        {
            RectTransform rect = appleCounterText.rectTransform;

            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            rect.anchoredPosition = new Vector2(0f, -285f);
            rect.sizeDelta = new Vector2(1400f, 340f);
        }

        if (gameOverText != null)
        {
            RectTransform rect = gameOverText.rectTransform;

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = new Vector2(0f, 130f);
            rect.sizeDelta = new Vector2(1080f, 260f);
        }

        if (unlockText != null)
        {
            RectTransform rect = unlockText.rectTransform;

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = new Vector2(0f, -30f);
            rect.sizeDelta = new Vector2(1050f, 190f);
        }

        if (backButton != null)
        {
            RectTransform rect =
                backButton.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);

            rect.anchoredPosition = new Vector2(-45f, -45f);
            rect.sizeDelta = new Vector2(300f, 115f);
        }
    }

    private void CreateUIIfMissing()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject =
                new GameObject("Canvas", typeof(RectTransform));

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1080, 1920);

            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (timerText == null)
        {
            timerText = CreateText(
                "TimerText",
                canvas.transform,
                "Time: 30",
                uiFontSize,
                uiTextScale
            );
        }

        if (appleCounterText == null)
        {
            appleCounterText = CreateText(
                "AppleCounterText",
                canvas.transform,
                "Apples: 0",
                uiFontSize,
                uiTextScale
            );
        }

        if (gameOverText == null)
        {
            gameOverText = CreateText(
                "GameOverText",
                canvas.transform,
                "Game Over!",
                messageFontSize,
                1f
            );
        }

        if (unlockText == null)
        {
            unlockText = CreateText(
                "UnlockText",
                canvas.transform,
                "You unlocked item!",
                unlockFontSize,
                1f
            );
        }

        if (backButton == null)
        {
            backButton = CreateButton(
                "BackButton",
                canvas.transform,
                "Back",
                buttonFontSize
            );
        }
    }

    private TextMeshProUGUI CreateText(
        string objectName,
        Transform parent,
        string textValue,
        float fontSize,
        float textScale
    )
    {
        GameObject textObject =
            new GameObject(objectName, typeof(RectTransform));

        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI uiText =
            textObject.AddComponent<TextMeshProUGUI>();

        uiText.text = textValue;
        uiText.alignment = TextAlignmentOptions.Center;

        ApplyAxolotl3DStyle(
            uiText,
            fontSize,
            textScale
        );

        return uiText;
    }

    private Button CreateButton(
        string objectName,
        Transform parent,
        string buttonTextValue,
        float textSize
    )
    {
        GameObject buttonObject =
            new GameObject(objectName, typeof(RectTransform));

        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.95f, 0.35f, 0.65f, 1f);

        Button button = buttonObject.AddComponent<Button>();

        TextMeshProUGUI buttonText = CreateText(
            "Text",
            buttonObject.transform,
            buttonTextValue,
            textSize,
            1f
        );

        RectTransform textRect = buttonText.rectTransform;

        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void GoBackToPlayroom()
    {
        SceneManager.LoadScene(playroomSceneName);
    }
}