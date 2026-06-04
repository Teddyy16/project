using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwiper : MonoBehaviour
{
    public string[] scenes = {
        "Dining room",
        "Playroom",
        "Main Room",
        "Supermarket",
        "Scanner"
    };

    private Vector2 startPos;
    private float minSwipeDistance = 400f; //Needs a bit of testing/adjusting
    private int currentIndex;

    void Start()
    {
        string current = SceneManager.GetActiveScene().name;
        currentIndex = System.Array.IndexOf(scenes, current);
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 endPos = touch.position;
            Vector2 swipe = endPos - startPos;

            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y) &&
                Mathf.Abs(swipe.x) > minSwipeDistance)
            {
                if (swipe.x > 0)
                    SwipeRight();
                else
                    SwipeLeft();
            }
        }
    }

    void SwipeLeft()
    {
        int next = (currentIndex + 1) % scenes.Length;
        SceneManager.LoadScene(scenes[next]);
    }

    void SwipeRight()
    {
        int next = (currentIndex - 1 + scenes.Length) % scenes.Length;
        SceneManager.LoadScene(scenes[next]);
    }
}

//NOTE:
//Work in Progress 
//Doesnt seem to work in every scene a bit buggy for now