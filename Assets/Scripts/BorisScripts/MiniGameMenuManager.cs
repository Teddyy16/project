using System.Collections;
using UnityEngine;
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

    private void Start()
    {
        SetupButtonClickAreas();

        if (miniGameMenuPanel != null)
        {
            miniGameMenuPanel.SetActive(false);
        }

        if (playButton != null)
        {
            playButton.SetActive(true);
        }
    }

    private void SetupButtonClickAreas()
    {
        if (miniGameMenuPanel == null)
        {
            return;
        }

        SetTransparentPartsNotClickable("MemoryCardGameButton");
        SetTransparentPartsNotClickable("Game2Button");
        SetTransparentPartsNotClickable("Game3Button");
        SetTransparentPartsNotClickable("CloseButton");
    }

    private void SetTransparentPartsNotClickable(string buttonObjectName)
    {
        Transform buttonTransform = miniGameMenuPanel.transform.Find(buttonObjectName);

        if (buttonTransform == null)
        {
            Debug.LogWarning("Could not find menu button: " + buttonObjectName);
            return;
        }

        Image buttonImage = buttonTransform.GetComponent<Image>();

        if (buttonImage == null)
        {
            Debug.LogWarning("No Image component on: " + buttonObjectName);
            return;
        }

        buttonImage.raycastTarget = true;
        buttonImage.alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OpenMenu()
    {
        if (isLoading)
        {
            return;
        }

        if (miniGameMenuPanel == null)
        {
            Debug.LogError("MiniGameMenuPanel is NOT assigned in Inspector!");
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

        if (miniGameMenuPanel != null)
        {
            miniGameMenuPanel.SetActive(false);
        }

        if (playButton != null)
        {
            playButton.SetActive(true);
        }
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

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}