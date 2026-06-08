using UnityEngine;

public class UIPanelManager : MonoBehaviour

 {
    public void ShowButton(GameObject button)
    {
        button.SetActive(true);
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }

    public void ToggleButton(GameObject button)
    {
        button.SetActive(!button.activeSelf);
    }
}

