using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text loadingText;
    [SerializeField] private float fakeLoadDelay = 2f;

    void Start()
    {
        StartCoroutine(LoadMainRoomAsync());
    }

    IEnumerator LoadMainRoomAsync()
    {
        yield return new WaitForSeconds(fakeLoadDelay);

        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Room");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = $"Loading... {Mathf.Floor(progress * 100)}%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
