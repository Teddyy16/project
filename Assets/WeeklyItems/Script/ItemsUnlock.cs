using System.Collections.Generic;
using UnityEngine;

public class ItemsUnlock : MonoBehaviour
{
    public WeeklyQuest weeklyQuest;

    [Header("Products")]
    public GameObject[] productObjects;

    [Header("Animal")]
    public GameObject animalToUnlock;

    private void Start()
    {
        UpdateUnlockedProducts();
        TryGiveAnimal();
    }

    public void UnlockRandomProduct()
    {
        if (weeklyQuest == null)
        {
            Debug.LogWarning("WeeklyQuest is missing.");
            return;
        }

        weeklyQuest.AddRandomIndex();

        UpdateUnlockedProducts();
        TryGiveAnimal();
    }

    public void TryGiveAnimal()
    {
        if (weeklyQuest == null || animalToUnlock == null)
        {
            return;
        }

        if (weeklyQuest.HasUnlockedThree())
        {
            animalToUnlock.SetActive(true);
        }
    }

    private void UpdateUnlockedProducts()
    {
        if (weeklyQuest == null || productObjects == null)
        {
            return;
        }

        List<int> unlockedIndexes = weeklyQuest.GetUnlockedIndexes();

        for (int i = 0; i < productObjects.Count; i++)
        {
            if (productObjects[i] != null)
            {
                productObjects[i].SetActive(unlockedIndexes.Contains(i));
            }
        }
    }
}