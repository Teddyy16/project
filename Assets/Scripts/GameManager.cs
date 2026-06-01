using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform blockPrefab;
    [SerializeField] private Transform blockHolder;

    private Transform currentBlock = null;
    private Rigidbody2D currentRigidbody;

    private Vector2 blockStartPosition = new Vector2(0f, 4f);

    private float blockSpeed = 8f;
    private float blockSpeedIncrement = 0.5f;
    private int blockDirection = 1;
    private float xLimit = 5f;

    void Start()
    {
        SpawnNewBlock();
    }

    private void SpawnNewBlock()
    {
        currentBlock = Instantiate(blockPrefab, blockHolder);
        currentBlock.position = blockStartPosition;
        currentBlock.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        currentRigidbody = currentBlock.GetComponent<Rigidbody2D>();

        blockSpeed += blockSpeedIncrement;
    }

    void Update()
    {
        if (currentBlock != null)
        {
            float moveAmount = Time.deltaTime * blockSpeed * blockDirection;
            currentBlock.position += new Vector3(moveAmount, 0, 0);

            if (Mathf.Abs(currentBlock.position.x) > xLimit)
            {
                currentBlock.position = new Vector3(xLimit * blockDirection, currentBlock.position.y, 0);
                blockDirection = -blockDirection;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentBlock != null)
            {
                currentBlock = null;
                currentRigidbody.simulated = true;
                StartCoroutine(DelayedSpawn());
            }
        }
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.8f);   // Small delay so the block can settle
        SpawnNewBlock();
    }
}