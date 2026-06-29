using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneChanger : MonoBehaviour
{
    [Header("Scene order")]
    [Tooltip("Write the scene names in the exact order you want to swipe through them.")]
    public string[] sceneNames;

    [Header("Swipe settings")]
    [Tooltip("Minimum horizontal swipe distance in pixels.")]
    public float minSwipeDistance = 80f;

    [Tooltip("Maximum vertical movement allowed, so vertical swipes do not change rooms.")]
    public float maxVerticalSwipe = 150f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private int currentSceneIndex = 0;
    private bool isLoadingScene = false;

    // Hrana blokira swipe-a za malko vreme.
    // Tova raboti dori ako Unity proveri swipe-a predi OnMouseUp.
    private static float blockSwipeUntilTime;

    public static void BlockSwipeForFood(float seconds = 0.6f)
    {
        float newBlockTime = Time.unscaledTime + seconds;

        if (newBlockTime > blockSwipeUntilTime)
        {
            blockSwipeUntilTime = newBlockTime;
        }
    }

    private static bool IsSwipeBlocked()
    {
        return Time.unscaledTime < blockSwipeUntilTime;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindCurrentSceneIndex();
        isLoadingScene = false;
    }

    private void Update()
    {
        DetectTouchSwipe();
        DetectMouseSwipeForUnityEditor();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoadingScene = false;
        blockSwipeUntilTime = 0f;

        FindCurrentSceneIndex();

        Debug.Log("Scene loaded: " + scene.name);
        Debug.Log("Current scene index: " + currentSceneIndex);
    }

    private void FindCurrentSceneIndex()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == currentSceneName)
            {
                currentSceneIndex = i;

                Debug.Log(
                    "Current scene found in array: " +
                    currentSceneName +
                    " at index " +
                    currentSceneIndex
                );

                return;
            }
        }

        Debug.LogWarning(
            "Current scene name was not found in Scene Names array: " +
            currentSceneName
        );
    }

    private void DetectTouchSwipe()
    {
        if (Input.touchCount == 0 || isLoadingScene)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }

        if (
            touch.phase == TouchPhase.Ended ||
            touch.phase == TouchPhase.Canceled
        )
        {
            endTouchPosition = touch.position;

            if (IsSwipeBlocked())
            {
                Debug.Log("Room swipe blocked because food was dragged.");
                return;
            }

            CheckSwipe(startTouchPosition, endTouchPosition);
        }
    }

    private void DetectMouseSwipeForUnityEditor()
    {
#if UNITY_EDITOR
        if (isLoadingScene)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;

            if (IsSwipeBlocked())
            {
                Debug.Log("Room swipe blocked because food was dragged.");
                return;
            }

            CheckSwipe(startTouchPosition, endTouchPosition);
        }
#endif
    }

    private void CheckSwipe(
        Vector2 startPosition,
        Vector2 endPosition
    )
    {
        if (IsSwipeBlocked())
        {
            return;
        }

        float horizontalSwipe = endPosition.x - startPosition.x;

        float verticalSwipe = Mathf.Abs(
            endPosition.y - startPosition.y
        );

        Debug.Log(
            "Swipe checked. Horizontal: " +
            horizontalSwipe +
            " Vertical: " +
            verticalSwipe
        );

        if (Mathf.Abs(horizontalSwipe) < minSwipeDistance)
        {
            return;
        }

        if (verticalSwipe > maxVerticalSwipe)
        {
            return;
        }

        if (horizontalSwipe < 0)
        {
            GoToNextScene();
        }
        else
        {
            GoToPreviousScene();
        }
    }

    public void GoToNextScene()
    {
        if (sceneNames == null || sceneNames.Length == 0)
        {
            return;
        }

        if (currentSceneIndex < sceneNames.Length - 1)
        {
            currentSceneIndex++;
            LoadSceneByIndex();
        }
        else
        {
            Debug.Log("Already at last scene.");
        }
    }

    public void GoToPreviousScene()
    {
        if (sceneNames == null || sceneNames.Length == 0)
        {
            return;
        }

        if (currentSceneIndex > 0)
        {
            currentSceneIndex--;
            LoadSceneByIndex();
        }
        else
        {
            Debug.Log("Already at first scene.");
        }
    }

    private void LoadSceneByIndex()
    {
        if (
            currentSceneIndex < 0 ||
            currentSceneIndex >= sceneNames.Length
        )
        {
            return;
        }

        string sceneToLoad = sceneNames[currentSceneIndex];

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogWarning(
                "Scene name is empty at index: " +
                currentSceneIndex
            );

            return;
        }

        Debug.Log("Loading scene: " + sceneToLoad);

        isLoadingScene = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}