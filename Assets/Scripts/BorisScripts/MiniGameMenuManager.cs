using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameMenuManager : MonoBehaviour
{
    [Header("Menu")]
    public GameObject miniGameMenuPanel;

    [Header("Main Play Button")]
    public GameObject playButton;

    [Header("Energy")]
    public float energyCost = 20f;
    public Text messageText;

    [Header("Scene Names")]
    public string memoryGameSceneName = "AxolotlMemoryGame";
    public string miniGame2SceneName = "MiniGame2";
    public string miniGame3SceneName = "FridgeStackingGame";

    private bool isLoading;
    private bool hasStarted;

    private void Awake()
    {
        HideMenuImmediately();
    }

    private void Start()
    {
        SetupMenuButtons();
        HideMenuImmediately();

        hasStarted = true;
    }

    private void OnEnable()
    {
        if (!hasStarted)
        {
            HideMenuImmediately();
        }
    }

    private void HideMenuImmediately()
    {
        if (miniGameMenuPanel != null)
        {
            miniGameMenuPanel.SetActive(false);
        }

        if (playButton != null)
        {
            playButton.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    private void SetupMenuButtons()
    {
        if (miniGameMenuPanel == null)
        {
            Debug.LogError("MiniGameMenuPanel is not assigned.");
            return;
        }

        SetupButton(
            "MemoryCardGameButton",
            OpenMemoryGame,
            new Vector2(0.78f, 0.30f)
        );

        SetupButton(
            "Game2Button",
            OpenMiniGame2,
            new Vector2(0.78f, 0.30f)
        );

        SetupButton(
            "Game3Button",
            OpenMiniGame3,
            new Vector2(0.78f, 0.30f)
        );

        SetupButton(
            "CloseButton",
            CloseMenu,
            new Vector2(0.65f, 0.30f)
        );
    }

    private void SetupButton(
        string buttonObjectName,
        UnityAction action,
        Vector2 visibleClickArea
    )
    {
        Transform buttonTransform =
            miniGameMenuPanel.transform.Find(buttonObjectName);

        if (buttonTransform == null)
        {
            Debug.LogWarning(
                "Could not find menu button: " + buttonObjectName
            );
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();

        if (button == null)
        {
            button = buttonTransform.GetComponentInChildren<Button>();
        }

        if (button == null)
        {
            Debug.LogWarning(
                "No Button component found on: " + buttonObjectName
            );
            return;
        }

        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.raycastTarget = true;
        }

        // Mahame greshni stari OnClick vruzki.
        button.onClick.RemoveAllListeners();

        // Slaga pravilnata igra za konkretniq buton.
        button.onClick.AddListener(action);

        // Butonut ostava 800 x 800 vizualno,
        // no samo sredata mu moje da prihvashta klik.
        VisibleButtonRaycastArea clickArea =
            button.GetComponent<VisibleButtonRaycastArea>();

        if (clickArea == null)
        {
            clickArea = button.gameObject.AddComponent<
                VisibleButtonRaycastArea
            >();
        }

        clickArea.SetClickArea(
            visibleClickArea.x,
            visibleClickArea.y
        );
    }

    public void OpenMenu()
    {
        if (isLoading)
        {
            return;
        }

        if (miniGameMenuPanel == null)
        {
            Debug.LogError(
                "MiniGameMenuPanel is NOT assigned in Inspector!"
            );
            return;
        }

        miniGameMenuPanel.SetActive(true);
        miniGameMenuPanel.transform.SetAsLastSibling();

        if (playButton != null)
        {
            playButton.SetActive(false);
        }

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void CloseMenu()
    {
        if (isLoading)
        {
            return;
        }

        HideMenuImmediately();
    }

    public void OpenMemoryGame()
    {
        TryOpenMiniGame(memoryGameSceneName);
    }

    public void OpenMiniGame2()
    {
        TryOpenMiniGame(miniGame2SceneName);
    }

    public void OpenMiniGame3()
    {
        TryOpenMiniGame(miniGame3SceneName);
    }

    private void TryOpenMiniGame(string sceneName)
    {
        if (isLoading)
        {
            return;
        }

        if (EnergyBar.Instance == null)
        {
            Debug.LogError("EnergyBar is missing from the scene.");
            return;
        }

        if (EnergyBar.Instance.TryUseEnergy(energyCost))
        {
            StartCoroutine(LoadMiniGameAsync(sceneName));
        }
        else
        {
            Debug.Log("Not enough energy!");

            if (messageText != null)
            {
                messageText.text = "Not enough energy!";
            }
        }
    }

    private IEnumerator LoadMiniGameAsync(string sceneName)
    {
        isLoading = true;

        if (messageText != null)
        {
            messageText.text = "Loading...";
        }

        AsyncOperation loadingOperation =
            SceneManager.LoadSceneAsync(sceneName);

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}

public class VisibleButtonRaycastArea :
    MonoBehaviour,
    ICanvasRaycastFilter
{
    private RectTransform rectTransform;

    private float widthPercent = 0.78f;
    private float heightPercent = 0.30f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetClickArea(
        float newWidthPercent,
        float newHeightPercent
    )
    {
        widthPercent = Mathf.Clamp01(newWidthPercent);
        heightPercent = Mathf.Clamp01(newHeightPercent);
    }

    public bool IsRaycastLocationValid(
        Vector2 screenPoint,
        Camera eventCamera
    )
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        Vector2 localPoint;

        bool isInsideRect =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPoint,
                eventCamera,
                out localPoint
            );

        if (!isInsideRect)
        {
            return false;
        }

        Rect fullRect = rectTransform.rect;

        float clickWidth = fullRect.width * widthPercent;
        float clickHeight = fullRect.height * heightPercent;

        Rect visibleButtonRect = new Rect(
            -clickWidth / 2f,
            -clickHeight / 2f,
            clickWidth,
            clickHeight
        );

        return visibleButtonRect.Contains(localPoint);
    }
}