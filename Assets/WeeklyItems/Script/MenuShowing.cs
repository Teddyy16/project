using UnityEngine;

public class MenuShowing : MonoBehaviour
{

    bool menushowing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(menushowing);


    }

    public void Toggle()
    {
        menushowing = !menushowing;
        gameObject.SetActive(menushowing);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
