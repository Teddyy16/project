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

    private void Start()
    {
        if (miniGameMenuPanel != null)
        {
            miniGameMenuPanel.SetActive(false);
        }

        if (playButton != null)
        {
            playButton.SetActive(true);
        }
    }

    public void OpenMenu()
    {
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
        if (EnergyBar.Instance == null)
        {
            Debug.LogError("EnergyBar is missing from the scene.");
            return;
        }

        if (EnergyBar.Instance.TryUseEnergy(energyCost))
        {
            SceneManager.LoadScene(sceneName);
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
}