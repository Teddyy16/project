using UnityEngine;

public class DraggableFood : MonoBehaviour
{
    [Header("Food")]
    public string foodName = "Apple";
    public int hungerAmount = 10;

    [Header("Feeding")]
    public float feedDistance = 5f;

    private Camera mainCamera;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Plane dragPlane;
    private Vector3 dragOffset;
    private bool isDragging;

    private PetMouth petMouth;

    private void Start()
    {
        mainCamera = Camera.main;

        startPosition = transform.position;
        startRotation = transform.rotation;

        petMouth = FindObjectOfType<PetMouth>();

        Debug.Log(
            "DraggableFood started: " +
            gameObject.name +
            " / FoodName: " +
            foodName
        );

        if (mainCamera == null)
        {
            Debug.LogError(
                "No Main Camera found. Make sure your camera has the MainCamera tag."
            );
        }

        if (petMouth == null)
        {
            Debug.LogWarning("PetMouth was not found in the scene.");
        }
        else
        {
            Debug.Log("PetMouth found: " + petMouth.gameObject.name);
        }

        if (PetNeedsManager.Instance == null)
        {
            Debug.LogWarning(
                "PetNeedsManager was not found. Hunger will not increase."
            );
        }
    }

    private void OnMouseDown()
    {
        if (mainCamera == null)
        {
            Debug.LogError(
                "Cannot drag because Main Camera is missing."
            );

            return;
        }

        // Vednaga blokira swipe za staq.
        SwipeSceneChanger.BlockSwipeForFood(2f);

        startPosition = transform.position;
        startRotation = transform.rotation;

        dragPlane = new Plane(
            -mainCamera.transform.forward,
            transform.position
        );

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            dragOffset = transform.position - hitPoint;
        }

        isDragging = true;

        Debug.Log("Clicked food: " + foodName);
    }

    private void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null)
        {
            return;
        }

        // Dokato vlachish, produljava da blokira swipe.
        SwipeSceneChanger.BlockSwipeForFood(2f);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + dragOffset;
        }
    }

    private void OnMouseUp()
    {
        // Ostava blokiran malko sled puskaneto,
        // za da ne se izpulni swipe v sushtiq frame.
        SwipeSceneChanger.BlockSwipeForFood(0.5f);

        isDragging = false;

        Debug.Log("Released food: " + foodName);

        if (IsCloseToMouth())
        {
            Debug.Log(
                "Food is close enough to PetMouth. Feeding..."
            );

            FeedPet();
        }
        else
        {
            Debug.Log(
                "Food is NOT close enough to PetMouth. Returning."
            );

            ReturnToStartPosition();
        }
    }

    private bool IsCloseToMouth()
    {
        if (petMouth == null)
        {
            petMouth = FindObjectOfType<PetMouth>();
        }

        if (petMouth == null)
        {
            Debug.LogWarning(
                "Cannot feed because PetMouth is missing."
            );

            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            petMouth.transform.position
        );

        Debug.Log("Distance to PetMouth: " + distance);

        return distance <= feedDistance;
    }

    private void FeedPet()
    {
        int amount = PlayerPrefs.GetInt(foodName, 0);

        Debug.Log(
            foodName +
            " amount before feeding: " +
            amount
        );

        if (amount <= 0)
        {
            Debug.LogWarning("No " + foodName + " left.");

            ReturnToStartPosition();
            return;
        }

        amount--;

        PlayerPrefs.SetInt(foodName, amount);
        PlayerPrefs.Save();

        Debug.Log(
            foodName +
            " amount after feeding: " +
            amount
        );

        if (PetNeedsManager.Instance != null)
        {
            PetNeedsManager.Instance.AddHunger(hungerAmount);

            Debug.Log(
                "Hunger increased by " +
                hungerAmount
            );
        }
        else
        {
            Debug.LogWarning(
                "PetNeedsManager was not found. Hunger was not increased."
            );
        }

        FridgeFoodVisibility fridgeFoodVisibility =
            FindObjectOfType<FridgeFoodVisibility>();

        if (fridgeFoodVisibility != null)
        {
            fridgeFoodVisibility.UpdateFridgeFood();

            Debug.Log("FridgeFoodVisibility updated.");
        }
        else
        {
            Debug.LogWarning(
                "FridgeFoodVisibility not found."
            );
        }

        FridgeInventoryUI fridgeInventoryUI =
            FindObjectOfType<FridgeInventoryUI>();

        if (fridgeInventoryUI != null)
        {
            fridgeInventoryUI.UpdateInventoryUI();

            Debug.Log("FridgeInventoryUI updated.");
        }
        else
        {
            Debug.LogWarning(
                "FridgeInventoryUI not found."
            );
        }

        if (amount <= 0)
        {
            Debug.Log(
                foodName +
                " is now 0, hiding object."
            );

            gameObject.SetActive(false);
        }
        else
        {
            ReturnToStartPosition();
        }

        Debug.Log("Pet successfully ate: " + foodName);
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        Debug.Log(
            "Returned " +
            foodName +
            " to fridge position."
        );
    }
}