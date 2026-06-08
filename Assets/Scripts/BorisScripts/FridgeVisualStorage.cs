using UnityEngine;

public class FridgeVisualStorage : MonoBehaviour
{
    public static FridgeVisualStorage Instance;

    [Header("Slot Root")]
    public Transform fridgeSlotsRoot;

    [Header("Slot Layout")]
    public float leftX = -0.25f;
    public float rightX = 0.25f;
    public float topY = 0.45f;
    public float rowSpacing = 0.25f;
    public float zPosition = 0f;

    [Header("Food Prefabs")]
    public GameObject applePrefab;
    public GameObject avocadoPrefab;
    public GameObject cheesePrefab;
    public GameObject milkPrefab;
    public GameObject carrotPrefab;
    public GameObject meatPrefab;
    public GameObject breadPrefab;
    public GameObject sodaPrefab;

    private int currentSlotIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("FRIDGE STARTED");
        Debug.Log("Saved Apple in fridge scene: " + PlayerPrefs.GetInt("Apple", 0));

        BuildFridgeFromSavedFood();
    }

    private void BuildFridgeFromSavedFood()
    {
        ClearFridge();

        currentSlotIndex = 0;

        AddSavedFood("Apple", PlayerPrefs.GetInt("Apple", 0));
        AddSavedFood("Avocado", PlayerPrefs.GetInt("Avocado", 0));
        AddSavedFood("Cheese", PlayerPrefs.GetInt("Cheese", 0));
        AddSavedFood("Milk", PlayerPrefs.GetInt("Milk", 0));
        AddSavedFood("Carrot", PlayerPrefs.GetInt("Carrot", 0));
        AddSavedFood("Meat", PlayerPrefs.GetInt("Meat", 0));
        AddSavedFood("Bread", PlayerPrefs.GetInt("Bread", 0));
        AddSavedFood("Soda", PlayerPrefs.GetInt("Soda", 0));
    }

    private void AddSavedFood(string foodName, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddFoodVisual(foodName);
        }
    }

    public void AddFoodVisual(string foodName)
    {
        if (fridgeSlotsRoot == null)
        {
            Debug.LogError("FridgeSlotsRoot is missing.");
            return;
        }

        if (currentSlotIndex >= 8)
        {
            Debug.Log("No free space in fridge!");
            return;
        }

        GameObject prefab = GetFoodPrefab(foodName);

        if (prefab == null)
        {
            Debug.LogWarning("No prefab found for food: " + foodName);
            return;
        }

        Vector3 localPosition = GetSlotLocalPosition(currentSlotIndex);

        GameObject foodObject = Instantiate(prefab, fridgeSlotsRoot);
        foodObject.transform.localPosition = localPosition;
        foodObject.transform.localRotation = Quaternion.identity;
        foodObject.transform.localScale = Vector3.one * 0.15f;

        Debug.Log("Spawned food in fridge: " + foodName + " at slot " + currentSlotIndex);

        currentSlotIndex++;
    }

    private Vector3 GetSlotLocalPosition(int index)
    {
        int row = index / 2;
        bool isLeft = index % 2 == 0;

        float x = isLeft ? leftX : rightX;
        float y = topY - row * rowSpacing;
        float z = zPosition;

        return new Vector3(x, y, z);
    }

    private void ClearFridge()
    {
        if (fridgeSlotsRoot == null) return;

        foreach (Transform child in fridgeSlotsRoot)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject GetFoodPrefab(string foodName)
    {
        if (foodName == "Apple") return applePrefab;
        if (foodName == "Avocado") return avocadoPrefab;
        if (foodName == "Cheese") return cheesePrefab;
        if (foodName == "Milk") return milkPrefab;
        if (foodName == "Carrot") return carrotPrefab;
        if (foodName == "Meat") return meatPrefab;
        if (foodName == "Bread") return breadPrefab;
        if (foodName == "Soda") return sodaPrefab;

        return null;
    }
}