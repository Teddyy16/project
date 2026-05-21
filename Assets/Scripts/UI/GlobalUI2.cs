using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUI2 : MonoBehaviour
{
    private static GlobalUI2 instance;

    [Header("UI Groups")]
    public GameObject coinsUI;     // always visible
    public GameObject statsUI;     // energy + hunger

    [Header("Scenes where stats should be hidden")]
    public string[] hideStatsInScenes;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Coins always visible
        coinsUI.SetActive(true);

        // Stats hidden only in specific scenes
        bool hideStats = false;

        foreach (string s in hideStatsInScenes)
        {
            if (scene.name == s)
            {
                hideStats = true;
                break;
            }
        }

        statsUI.SetActive(!hideStats);
    }
}