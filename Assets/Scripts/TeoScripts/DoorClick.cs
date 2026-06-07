using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false; // Tracks if the door is currently open

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (anim != null)
        {
            if (isOpen)
            {
                anim.SetTrigger("DoorClose");
                isOpen = false;
                 Debug.Log("aufnbaief");
            }
            else
            {
                anim.SetTrigger("DoorClose");
                isOpen = true; 
            }
        }
    }   

}