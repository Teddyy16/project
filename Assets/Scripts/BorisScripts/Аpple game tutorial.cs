using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppleStartTutorial : MonoBehaviour
{
    [Header("References")]
    public AppleGameManager gameManager;
    public Canvas canvas;

    [Header("Text Style")]
    public TMP_FontAsset gameFont;

    [Header("Tutorial Text")]
    public string instructionText = "Move left and right!";
    public string startText = "Tap anywhere to start";

    private GameObject tutorialPanel;
    private bool tutorialShowing = true;

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<AppleGameManager>();
        }

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        CreateTutorialPanel();

        if (gameManager != null)
        {
            gameManager.enabled = false;
        }
    }

    private void CreateTutorialPanel()
    {
        if (canvas == null)
        {
            Debug.LogError("No Canvas found for AppleStartTutorial.");
            return;
        }

        tutorialPanel = new GameObject(
            "AppleStartTutorialPanel",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        tutorialPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect =
            tutorialPanel.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = tutorialPanel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.45f);

        Button panelButton = tutorialPanel.GetComponent<Button>();
        panelButton.onClick.AddListener(StartGame);

        CreateText(
            "InstructionText",
            instructionText,
            new Vector2(0f, 290f),
            new Vector2(1000f, 150f),
            90f
        );

        CreateText(
            "LeftArrow",
            "←",
            new Vector2(-260f, 40f),
            new Vector2(300f, 300f),
            270f
        );

        CreateText(
            "RightArrow",
            "→",
            new Vector2(260f, 40f),
            new Vector2(300f, 300f),
            270f
        );

        CreateText(
            "StartText",
            startText,
            new Vector2(0f, -270f),
            new Vector2(1000f, 130f),
            58f
        );
    }

    private void CreateText(
        string objectName,
        string textValue,
        Vector2 position,
        Vector2 size,
        float fontSize
    )
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform)
        );

        textObject.transform.SetParent(tutorialPanel.transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI text =
            textObject.AddComponent<TextMeshProUGUI>();

        text.text = textValue;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        text.enableAutoSizing = false;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;

        if (gameFont != null)
        {
            text.font = gameFont;
        }

        Material material = new Material(text.fontSharedMaterial);

        material.EnableKeyword("UNDERLAY_ON");

        material.SetColor(
            ShaderUtilities.ID_FaceColor,
            Color.white
        );

        material.SetFloat(
            ShaderUtilities.ID_OutlineWidth,
            0.025f
        );

        material.SetColor(
            ShaderUtilities.ID_OutlineColor,
            new Color(0.85f, 0.95f, 1f, 1f)
        );

        material.SetColor(
            ShaderUtilities.ID_UnderlayColor,
            new Color(0.2f, 0.42f, 0.9f, 1f)
        );

        material.SetFloat(
            ShaderUtilities.ID_UnderlayOffsetX,
            0.01f
        );

        material.SetFloat(
            ShaderUtilities.ID_UnderlayOffsetY,
            -0.13f
        );

        material.SetFloat(
            ShaderUtilities.ID_UnderlayDilate,
            0.12f
        );

        material.SetFloat(
            ShaderUtilities.ID_UnderlaySoftness,
            0f
        );

        text.fontMaterial = material;
        text.UpdateMeshPadding();
    }

    private void StartGame()
    {
        if (!tutorialShowing)
        {
            return;
        }

        tutorialShowing = false;

        Time.timeScale = 1f;

        if (tutorialPanel != null)
        {
            Destroy(tutorialPanel);
        }

        if (gameManager != null)
        {
            gameManager.enabled = true;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}