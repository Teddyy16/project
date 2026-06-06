using UnityEngine;

public class FallingMilk : MonoBehaviour
{

    private GameObject milk;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(col.gameObject);
        }

        else
        {

        }
    }
}
