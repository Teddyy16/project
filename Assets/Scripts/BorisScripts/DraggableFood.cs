using UnityEngine;

public class DraggableFood : MonoBehaviour
{
    [Header("Food")]
    public string foodName = "Apple";
    public int energyAmount = 10;

    [Header("References")]
    public EnergyBar energyBar;

    [Header("Feeding")]
    public float feedDistance = 5f;

    private Camera mainCamera;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Plane dragPlane;
    private Vector3 dragOffset;
    private bool isDragging;

    private PetMouth petMouth;

    void Start()
    {
        mainCamera = Camera.main;

        startPosition = transform.position;
        startRotation = transform.rotation;

        petMouth = FindObjectOfType<PetMouth>();

        if (energyBar == null)
        {
            energyBar = FindObjectOfType<EnergyBar>();
        }

        Debug.Log("DraggableFood started: " + gameObject.name + " / FoodName: " + foodName);

        if (petMouth == null)
        {
            Debug.LogWarning("PetMouth was not found in the scene.");
        }
        else
        {
            Debug.Log("PetMouth found: " + petMouth.gameObject.name);
        }

        if (energyBar == null)
        {
            Debug.LogWarning("EnergyBar was not found. Food can be eaten, but energy may not increase.");
        }
    }

    void OnMouseDown()
    {
        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found.");
            return;
        }

        startPosition = transform.position;
        startRotation = transform.rotation;

        dragPlane = new Plane(-mainCamera.transform.forward, transform.position);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            dragOffset = transform.position - hitPoint;
        }

        isDragging = true;

        Debug.Log("Clicked food: " + foodName);
    }

    void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + dragOffset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        Debug.Log("Released food: " + foodName);

        if (IsCloseToMouth())
        {
            Debug.Log("Food is close enough to PetMouth. Feeding...");
            FeedPet();
        }
        else
        {
            Debug.Log("Food is NOT close enough to PetMouth. Returning.");
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
            Debug.LogWarning("Cannot feed because PetMouth is missing.");
            return false;
        }

        float distance = Vector3.Distance(transform.position, petMouth.transform.position);

        Debug.Log("Distance to PetMouth: " + distance);

        return distance <= feedDistance;
    }

    private void FeedPet()
    {
        int amount = PlayerPrefs.GetInt(foodName, 0);

        Debug.Log(foodName + " amount before feeding: " + amount);

        if (amount <= 0)
        {
            Debug.LogWarning("No " + foodName + " left.");
            ReturnToStartPosition();
            return;
        }

        amount--;

        PlayerPrefs.SetInt(foodName, amount);
        PlayerPrefs.Save();

        Debug.Log(foodName + " amount after feeding: " + amount);

        if (energyBar != null)
        {
            energyBar.AddEnergy(energyAmount);
            Debug.Log("Energy increased by " + energyAmount);
        }
        else if (EnergyBar.Instance != null)
        {
            EnergyBar.Instance.AddEnergy(energyAmount);
            Debug.Log("Energy increased by " + energyAmount + " using EnergyBar.Instance");
        }
        else
        {
            Debug.LogWarning("EnergyBar missing, energy was not increased.");
        }

        FridgeFoodVisibility fridgeFoodVisibility = FindObjectOfType<FridgeFoodVisibility>();

        if (fridgeFoodVisibility != null)
        {
            fridgeFoodVisibility.UpdateFridgeFood();
        }

        FridgeInventoryUI fridgeInventoryUI = FindObjectOfType<FridgeInventoryUI>();

        if (fridgeInventoryUI != null)
        {
            fridgeInventoryUI.UpdateInventoryUI();
        }

        if (amount <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            ReturnToStartPosition();
        }

        Debug.Log("Pet ate: " + foodName);
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}