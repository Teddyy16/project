using System.Collections.Generic;
using UnityEngine;

public class FridgeFoodVisibility : MonoBehaviour
{
    [Header("Food Objects In Fridge")]
    public GameObject appleObject;
    public GameObject avocadoObject;
    public GameObject cheeseObject;
    public GameObject milkObject;
    public GameObject carrotObject;
    public GameObject meatObject;
    public GameObject breadObject;
    public GameObject sodaObject;

    private Dictionary<string, GameObject> foodObjects;

    void Start()
    {
        foodObjects = new Dictionary<string, GameObject>();

        foodObjects.Add("Apple", appleObject);
        foodObjects.Add("Avocado", avocadoObject);
        foodObjects.Add("Cheese", cheeseObject);
        foodObjects.Add("Milk", milkObject);
        foodObjects.Add("Carrot", carrotObject);
        foodObjects.Add("Meat", meatObject);
        foodObjects.Add("Bread", breadObject);
        foodObjects.Add("Soda", sodaObject);

        UpdateFridgeFood();
    }

    public void UpdateFridgeFood()
    {
        foreach (KeyValuePair<string, GameObject> food in foodObjects)
        {
            string foodName = food.Key;
            GameObject foodObject = food.Value;

            int amount = PlayerPrefs.GetInt(foodName, 0);
            bool hasFood = amount > 0;

            if (foodObject != null)
            {
                foodObject.SetActive(hasFood);
            }

            Debug.Log(foodName + " amount in fridge: " + amount);
        }
    }
}