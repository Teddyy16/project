using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AxolotlMemoryGame : MonoBehaviour
{
    [Header("Game")]
    public int columns = 4;
    public float flipBackDelay = 0.75f;

    public WeeklyQuest weeklyquest;

    [Header("Scene Names")]
    public string playroomSceneName = "Playroom";

    [Header("Optional UI")]
    public Text titleText;
    public Text resultText;
    public Text movesText;
    public Text timerText;
    public Text bestTimeText;

    [Header("3D Text Settings")]
    public Color normalTextColor = Color.white;
    public Color shadowColor = new Color(0f, 0f, 0f, 0.55f);
    public Vector2 shadowOffset = new Vector2(5f, -6f);

    private readonly string bestTimeKey = "AxolotlMemoryBestTime";

    private readonly string[] frontSpriteNames = new string[]
    {
        "axolotl_standing",
        "axolotl_cave",
        "axolotl_moss_ball",
        "bubble",
        "seaweed",
        "driftwood",
        "axolotl_pot",
        "shrimp"
    };

    private Sprite backSprite;
    private List<Sprite> frontSprites = new List<Sprite>();
    private List<CardView> cards = new List<CardView>();

    private CardView firstCard;
    private CardView secondCard;

    private bool locked;
    private int matchesFound;
    private int moves;

    private Transform gridTransform;
    private Font font;

    private float timer;
    private bool timerRunning;
    private float bestTime;

    private class CardView
    {
        public Button button;
        public Image image;
        public Sprite front;
        public Sprite back;
        public bool flipped;
        public bool matched;
    }

    private void Start()
    {
        font = Resources.Load<Font>("Fonts/Franklin Gothic Heavy Regular");

        if (font == null)
        {
            Debug.LogError(
                "Franklin Gothic Heavy Regular was not found. " +
                "Put the font in Assets/Resources/Fonts/Franklin Gothic Heavy Regular.ttf"
            );

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        LoadSprites();
        BuildInterface();
        StartNewGame();
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void LoadSprites()
    {
        backSprite = Resources.Load<Sprite>("AxolotlCards/card_back");
        frontSprites.Clear();

        foreach (string spriteName in frontSpriteNames)
        {
            Sprite sprite = Resources.Load<Sprite>("AxolotlCards/" + spriteName);

            if (sprite != null)
            {
                frontSprites.Add(sprite);
            }
            else
            {
                Debug.LogWarning(
                    "Missing sprite in Resources/AxolotlCards: " + spriteName
                );
            }
        }

        if (backSprite == null)
        {
            Debug.LogError(
                "Missing card_back sprite. Check Assets/AxolotlMemoryGame/Resources/AxolotlCards/card_back.png"
            );
        }
    }

    private void BuildInterface()
    {
        EnsureEventSystem();

        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject(
                "Canvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();

        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        GameObject root = CreateUIObject("Axolotl Memory Root", canvas.transform);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image background = root.AddComponent<Image>();
        background.color = new Color(0.90f, 0.96f, 1f, 1f);

        titleText = CreateText(
            "Title",
            root.transform,
            "Axolotl Memory",
            70,
            TextAnchor.MiddleCenter
        );

        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -35);
        titleRect.sizeDelta = new Vector2(950, 85);

        movesText = CreateText(
            "Moves",
            root.transform,
            "Moves: 0",
            44,
            TextAnchor.MiddleCenter
        );

        RectTransform movesRect = movesText.GetComponent<RectTransform>();
        movesRect.anchorMin = new Vector2(0.5f, 1f);
        movesRect.anchorMax = new Vector2(0.5f, 1f);
        movesRect.pivot = new Vector2(0.5f, 1f);
        movesRect.anchoredPosition = new Vector2(0, -115);
        movesRect.sizeDelta = new Vector2(900, 55);

        timerText = CreateText(
            "Timer",
            root.transform,
            "Time: 00:00",
            44,
            TextAnchor.MiddleCenter
        );

        RectTransform timerRect = timerText.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.5f, 1f);
        timerRect.anchorMax = new Vector2(0.5f, 1f);
        timerRect.pivot = new Vector2(0.5f, 1f);
        timerRect.anchoredPosition = new Vector2(0, -170);
        timerRect.sizeDelta = new Vector2(900, 55);

        bestTimeText = CreateText(
            "BestTime",
            root.transform,
            "Best: --:--",
            38,
            TextAnchor.MiddleCenter
        );

        RectTransform bestRect = bestTimeText.GetComponent<RectTransform>();
        bestRect.anchorMin = new Vector2(0.5f, 1f);
        bestRect.anchorMax = new Vector2(0.5f, 1f);
        bestRect.pivot = new Vector2(0.5f, 1f);
        bestRect.anchoredPosition = new Vector2(0, -220);
        bestRect.sizeDelta = new Vector2(900, 50);

        GameObject grid = CreateUIObject("CardGrid", root.transform);
        gridTransform = grid.transform;

        RectTransform gridRect = grid.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.pivot = new Vector2(0.5f, 0.5f);
        gridRect.anchoredPosition = new Vector2(0, -250);
        gridRect.sizeDelta = new Vector2(900, 900);

        GridLayoutGroup gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(170, 220);
        gridLayout.spacing = new Vector2(18, 18);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        resultText = CreateText(
            "Result",
            root.transform,
            "",
            56,
            TextAnchor.MiddleCenter
        );

        RectTransform resultRect = resultText.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0.5f, 0f);
        resultRect.anchorMax = new Vector2(0.5f, 0f);
        resultRect.pivot = new Vector2(0.5f, 0f);
        resultRect.anchoredPosition = new Vector2(0, 150);
        resultRect.sizeDelta = new Vector2(1000, 190);

        Button backButton = CreateButton(
            "BackButton",
            root.transform,
            "Back",
            42
        );

        backButton.onClick.AddListener(GoBackToPlayroom);

        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0f);
        backRect.anchorMax = new Vector2(0.5f, 0f);
        backRect.pivot = new Vector2(0.5f, 0f);
        backRect.anchoredPosition = new Vector2(-180, 50);
        backRect.sizeDelta = new Vector2(300, 80);

        Button restartButton = CreateButton(
            "RestartButton",
            root.transform,
            "Restart",
            42
        );

        restartButton.onClick.AddListener(StartNewGame);

        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0f);
        restartRect.anchorMax = new Vector2(0.5f, 0f);
        restartRect.pivot = new Vector2(0.5f, 0f);
        restartRect.anchoredPosition = new Vector2(180, 50);
        restartRect.sizeDelta = new Vector2(300, 80);
    }

    private void StartNewGame()
    {
        if (gridTransform == null || frontSprites.Count == 0 || backSprite == null)
        {
            return;
        }

        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }

        cards.Clear();

        firstCard = null;
        secondCard = null;

        locked = false;
        matchesFound = 0;
        moves = 0;

        timer = 0f;
        timerRunning = true;

        bestTime = PlayerPrefs.GetFloat(bestTimeKey, 0f);

        UpdateTexts();
        UpdateTimerText();
        UpdateBestTimeText();

        List<Sprite> deck = new List<Sprite>();

        foreach (Sprite sprite in frontSprites)
        {
            deck.Add(sprite);
            deck.Add(sprite);
        }

        Shuffle(deck);

        foreach (Sprite sprite in deck)
        {
            CreateCard(sprite);
        }
    }

    private void GoBackToPlayroom()
    {
        SceneManager.LoadScene(playroomSceneName);
    }

    private void CreateCard(Sprite front)
    {
        GameObject cardObject = new GameObject(
            "Card",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        cardObject.transform.SetParent(gridTransform, false);

        Image image = cardObject.GetComponent<Image>();
        image.sprite = backSprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        Button button = cardObject.GetComponent<Button>();

        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.95f, 0.95f, 1f, 1f);
        colors.pressedColor = new Color(0.85f, 0.90f, 1f, 1f);
        button.colors = colors;

        CardView card = new CardView
        {
            button = button,
            image = image,
            front = front,
            back = backSprite,
            flipped = false,
            matched = false
        };

        button.onClick.AddListener(() => OnCardPressed(card));

        cards.Add(card);
    }

    private void OnCardPressed(CardView card)
    {
        if (locked || card.flipped || card.matched)
        {
            return;
        }

        FlipOpen(card);

        if (firstCard == null)
        {
            firstCard = card;
            return;
        }

        secondCard = card;
        moves++;

        UpdateTexts();

        StartCoroutine(CheckPair());
    }

    private IEnumerator CheckPair()
    {
        locked = true;

        yield return new WaitForSeconds(flipBackDelay);

        if (firstCard.front == secondCard.front)
        {
            firstCard.matched = true;
            secondCard.matched = true;

            firstCard.button.interactable = false;
            secondCard.button.interactable = false;

            matchesFound++;

            if (matchesFound >= frontSprites.Count)
            {
                WinGame();
            }
        }
        else
        {
            FlipBack(firstCard);
            FlipBack(secondCard);
        }

        firstCard = null;
        secondCard = null;
        locked = false;
    }

    private void WinGame()
    {
        if (weeklyquest != null)
        {
            weeklyquest.AddRandomIndex();
        }
        else
        {
            Debug.LogWarning("WeeklyQuest is not assigned.");
        }

        timerRunning = false;

        bool newBestScore = false;

        if (bestTime == 0f || timer < bestTime)
        {
            bestTime = timer;

            PlayerPrefs.SetFloat(bestTimeKey, bestTime);
            PlayerPrefs.Save();

            newBestScore = true;
        }

        string finalTime = FormatTime(timer);

        if (newBestScore)
        {
            resultText.text =
                "You won!\n" +
                "Time: " + finalTime + "\n" +
                "NEW BEST SCORE!\n" +
                "New item unlocked!";
        }
        else
        {
            resultText.text =
                "You won!\n" +
                "Time: " + finalTime + "\n" +
                "New item unlocked!";
        }

        UpdateBestTimeText();

        Debug.Log("Axolotl Memory completed. Time: " + finalTime);
        Debug.Log("New item unlocked!");
    }

    private void FlipOpen(CardView card)
    {
        card.flipped = true;
        card.image.sprite = card.front;
    }

    private void FlipBack(CardView card)
    {
        card.flipped = false;
        card.image.sprite = card.back;
    }

    private void UpdateTexts()
    {
        if (movesText != null)
        {
            movesText.text = "Moves: " + moves;
        }

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + FormatTime(timer);
        }
    }

    private void UpdateBestTimeText()
    {
        if (bestTimeText != null)
        {
            if (bestTime <= 0f)
            {
                bestTimeText.text = "Best: --:--";
            }
            else
            {
                bestTimeText.text = "Best: " + FormatTime(bestTime);
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);

        return obj;
    }

    private Text CreateText(
        string objectName,
        Transform parent,
        string text,
        int size,
        TextAnchor anchor
    )
    {
        GameObject obj = CreateUIObject(objectName, parent);

        Text uiText = obj.AddComponent<Text>();

        uiText.font = font;
        uiText.text = text;
        uiText.fontSize = size;
        uiText.alignment = anchor;
        uiText.color = normalTextColor;
        uiText.fontStyle = FontStyle.Bold;

        uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
        uiText.verticalOverflow = VerticalWrapMode.Overflow;

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowOffset;
        shadow.useGraphicAlpha = true;

        return uiText;
    }

    private Button CreateButton(
        string objectName,
        Transform parent,
        string text,
        int textSize
    )
    {
        GameObject obj = CreateUIObject(objectName, parent);

        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.56f, 0.70f, 0.95f, 1f);

        Button button = obj.AddComponent<Button>();

        Text buttonText = CreateText(
            "Text",
            obj.transform,
            text,
            textSize,
            TextAnchor.MiddleCenter
        );

        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        buttonText.color = Color.white;

        return button;
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule)
            );

            DontDestroyOnLoad(eventSystem);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}