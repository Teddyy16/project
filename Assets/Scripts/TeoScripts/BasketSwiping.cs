 using UnityEngine;

public class BasketSwiper : MonoBehaviour
{
    [Header("Movement Settings")]
    public float basketSpacing = -2f;       // The exact distance between each basket along the X-axis
    public float swipeThreshold = 100f;     // How many pixels the finger must move to count as a swipe
    public float smoothSpeed = 10f;        // How fast the carousel slides into position

    private int currentBasketIndex = 0;    // Index of the basket currently in front of the camera
    public int totalBaskets = 2;          // Change this to match how many baskets you have
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private Vector3 targetPosition;

    void Start()
    {
        // Set our initial target position to wherever the carousel starts
        targetPosition = transform.position;
    }

    void Update()
    {
        float mouseTouchY = Input.mousePosition.y;

    // Screen.height * 0.25f means the bottom 25% of the player's screen
    // If the touch is higher up where the food is dragged, ignore it!
    if (mouseTouchY > Screen.height * 0.25f)
    {
        return; 
    }
        HandleMouseOrTouchInput();
        
        // Smoothly slide the carousel towards the target position every frame
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    void HandleMouseOrTouchInput()
    {
        // 1. Detect when the player first clicks / touches the screen
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }

        // 2. Detect when they lift their finger / release the click
        if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Input.mousePosition;
            AnalyzeSwipe();
        }
    }

   void AnalyzeSwipe()
    {
        float swipeDistanceX = touchEndPos.x - touchStartPos.x;

        // Check if the movement was large enough to be considered an intentional swipe
        if (Mathf.Abs(swipeDistanceX) > swipeThreshold)
        {
            // SWAP THE DIRECTIONS HERE BECAUSE OF YOUR 180-DEGREE ROTATION
            if (swipeDistanceX < 0)
            {
                // Swiped Left -> Move to the PREVIOUS basket (Index decreases)
                MoveToBasket(currentBasketIndex - 1);
            }
            else if (swipeDistanceX > 0)
            {
                // Swiped Right -> Move to the NEXT basket (Index increases)
                MoveToBasket(currentBasketIndex + 1);
            }
        }
    }

    void MoveToBasket(int newIndex)
    {
        // Clamp the index so the player can't swipe past the first or last basket
        currentBasketIndex = Mathf.Clamp(newIndex, 0, totalBaskets - 1);

        // FLIP THE SIGN HERE: 
        // Because your second basket is at -1, we multiply by positive currentBasketIndex
        // instead of negative to slide it in the opposite direction.
        float newX = currentBasketIndex * basketSpacing;
        
        targetPosition = new Vector3(newX, transform.position.y, transform.position.z);
    }
}