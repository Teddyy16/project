using UnityEngine;

public class ClickAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        
        anim = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        // this function automatically runs when the collider is clicked
        if (anim != null)
        {
            anim.SetTrigger("PlayNod");
        }
    }
}
