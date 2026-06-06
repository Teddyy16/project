
using UnityEngine;

public class Falling : MonoBehaviour
{

   
    private GameObject Apple_Final;
   
   

    void Start()
    {
      
    }


    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
