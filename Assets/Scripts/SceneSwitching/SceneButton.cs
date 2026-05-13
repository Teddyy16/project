using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string sceneName;
    public Color activeColor = Color.green;
    public Color normalColor = Color.white;

    Button button;
    Image image;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        // Add the OnClick event IN CODE
        button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }

    void Update()
    {
        bool isCurrent = SceneManager.GetActiveScene().name == sceneName;

        button.interactable = !isCurrent;
        image.color = isCurrent ? activeColor : normalColor;
    }
}