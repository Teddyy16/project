using System.Collections.Generic;
using UnityEngine;

public class BasketSpawner : MonoBehaviour
{
    [Header("Food Prefabs to Choose From")]
    public List<GameObject> foodPrefabs; // List for food items to drag in Unity

    [Header("Spawn Settings")]
    public int numberOfItemsToSpawn = 5;
    public List<Transform> spawnPoints;  // Empty GameObjects placed inside the basket that act as spawn points

    void Start()
    {
        SpawnFoodInBasket();
    }

    void SpawnFoodInBasket()
    {
        // Safety check
        if (foodPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        // Spawn items up to a chosen limit and then stop
        for (int i = 0; i < numberOfItemsToSpawn; i++)
        {
            // If there are no spawn points avaible, stop spawning
            if (i >= spawnPoints.Count) break; 

            
            int randomIndex = Random.Range(0, foodPrefabs.Count);
            GameObject chosenFood = foodPrefabs[randomIndex];

            // Spawns the food at a specific spawn point
            GameObject spawnedFood = Instantiate(chosenFood, spawnPoints[i].position, spawnPoints[i].rotation);

            // The food becomes child of the basket so they move tgt
            spawnedFood.transform.SetParent(this.transform);
        }
    }
}
