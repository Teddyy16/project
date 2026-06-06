using UnityEngine;

public class FridgeVisualStorage : MonoBehaviour
{
    public static FridgeVisualStorage Instance;

    [Header("Food Slots")]
    public Transform[] foodSlots;

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
        if (currentSlotIndex >= foodSlots.Length)
        {
            Debug.Log("No free space in fridge!");
            return;
        }

        Transform slot = foodSlots[currentSlotIndex];

        if (slot == null)
        {
            currentSlotIndex++;
            return;
        }

        GameObject prefab = GetFoodPrefab(foodName);

        if (prefab == null)
        {
            Debug.LogWarning("No prefab found for food: " + foodName);
            return;
        }

        GameObject foodObject = Instantiate(prefab, slot.position, slot.rotation);
        foodObject.transform.SetParent(slot);
        foodObject.transform.localPosition = Vector3.zero;
        foodObject.transform.localRotation = Quaternion.identity;

        currentSlotIndex++;
    }

    private void ClearFridge()
    {
        foreach (Transform slot in foodSlots)
        {
            if (slot == null) continue;

            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
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