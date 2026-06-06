using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public bool givesPoint = false;
    public bool causesGameOver = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (causesGameOver && AppleGameManager.Instance != null)
            {
                AppleGameManager.Instance.GameOver();
            }

            if (givesPoint && AppleGameManager.Instance != null)
            {
                AppleGameManager.Instance.AddApple();
            }

            Destroy(gameObject);
        }
    }
}