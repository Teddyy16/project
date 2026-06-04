using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AxolotlMemoryGame : MonoBehaviour
{
    [Header("Game")]
    public int columns = 4;
    public float flipBackDelay = 0.75f;
    public int rewardCoins = 50;

    [Header("Optional UI")]
    public Text titleText;
    public Text resultText;
    public Text movesText;

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
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        LoadSprites();
        BuildInterface();
        StartNewGame();
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
                Debug.LogWarning("Missing sprite in Resources/AxolotlCards: " + spriteName);
            }
        }

        if (backSprite == null)
        {
            Debug.LogError("Missing card_back sprite. Check Assets/AxolotlMemoryGame/Resources/AxolotlCards/card_back.png");
        }
    }

    private void BuildInterface()
    {
        EnsureEventSystem();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (canvas.GetComponent<GraphicRaycaster>() == null)
            canvas.gameObject.AddComponent<GraphicRaycaster>();

        GameObject root = CreateUIObject("Axolotl Memory Root", canvas.transform);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image background = root.AddComponent<Image>();
        background.color = new Color(0.90f, 0.96f, 1f, 1f);

        titleText = CreateText("Title", root.transform, "Axolotl Memory", 58, TextAnchor.MiddleCenter);
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -90);
        titleRect.sizeDelta = new Vector2(900, 100);

        movesText = CreateText("Moves", root.transform, "Moves: 0", 34, TextAnchor.MiddleCenter);
        RectTransform movesRect = movesText.GetComponent<RectTransform>();
        movesRect.anchorMin = new Vector2(0.5f, 1f);
        movesRect.anchorMax = new Vector2(0.5f, 1f);
        movesRect.pivot = new Vector2(0.5f, 1f);
        movesRect.anchoredPosition = new Vector2(0, -170);
        movesRect.sizeDelta = new Vector2(900, 70);

        GameObject grid = CreateUIObject("CardGrid", root.transform);
        gridTransform = grid.transform;
        RectTransform gridRect = grid.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.pivot = new Vector2(0.5f, 0.5f);
        gridRect.anchoredPosition = new Vector2(0, -60);
        gridRect.sizeDelta = new Vector2(900, 1180);

        GridLayoutGroup gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(190, 260);
        gridLayout.spacing = new Vector2(22, 22);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        resultText = CreateText("Result", root.transform, "", 40, TextAnchor.MiddleCenter);
        RectTransform resultRect = resultText.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0.5f, 0f);
        resultRect.anchorMax = new Vector2(0.5f, 0f);
        resultRect.pivot = new Vector2(0.5f, 0f);
        resultRect.anchoredPosition = new Vector2(0, 150);
        resultRect.sizeDelta = new Vector2(900, 100);

        Button restartButton = CreateButton("RestartButton", root.transform, "Restart", 34);
        restartButton.onClick.AddListener(StartNewGame);
        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0f);
        restartRect.anchorMax = new Vector2(0.5f, 0f);
        restartRect.pivot = new Vector2(0.5f, 0f);
        restartRect.anchoredPosition = new Vector2(0, 50);
        restartRect.sizeDelta = new Vector2(300, 80);
    }

    private void StartNewGame()
    {
        if (gridTransform == null || frontSprites.Count == 0 || backSprite == null) return;

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
        UpdateTexts();

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

    private void CreateCard(Sprite front)
    {
        GameObject cardObject = new GameObject("Card", typeof(RectTransform), typeof(Image), typeof(Button));
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
        if (locked || card.flipped || card.matched) return;

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
                resultText.text = "You won! +" + rewardCoins + " coins";
                Debug.Log("Axolotl Memory completed. Reward coins: " + rewardCoins);
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
        if (movesText != null) movesText.text = "Moves: " + moves;
        if (resultText != null) resultText.text = "";
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private Text CreateText(string objectName, Transform parent, string text, int size, TextAnchor anchor)
    {
        GameObject obj = CreateUIObject(objectName, parent);
        Text uiText = obj.AddComponent<Text>();
        uiText.font = font;
        uiText.text = text;
        uiText.fontSize = size;
        uiText.alignment = anchor;
        uiText.color = new Color(0.18f, 0.15f, 0.22f, 1f);
        return uiText;
    }

    private Button CreateButton(string objectName, Transform parent, string text, int textSize)
    {
        GameObject obj = CreateUIObject(objectName, parent);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.56f, 0.70f, 0.95f, 1f);
        Button button = obj.AddComponent<Button>();

        Text buttonText = CreateText("Text", obj.transform, text, textSize, TextAnchor.MiddleCenter);
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
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
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
