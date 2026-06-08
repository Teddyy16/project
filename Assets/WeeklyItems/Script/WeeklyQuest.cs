using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyQuest", menuName = "Scriptable Objects/WeeklyQuest")]
public class WeeklyQuest : ScriptableObject
{
    public int maxIcons=3;
    public List<Sprite> foodIcons;

    public List<int> unlocked ;

    private void OnEnable()
    {
        if (unlocked == null)
        {
            unlocked = new List<int>();
        }

        if (unlocked.Count == 0)
        {
            AddRandomIndex();
        }
    }

    public void AddRandomIndex()
    { Debug.Log("----------------------------1------------");
        if (foodIcons == null || foodIcons.Count == 0)
        {
            Debug.LogWarning("WeeklyQuest has no food icons.");
            return;
        }
        Debug.Log("-------------------------------2---------");

        if (unlocked.Count >= maxIcons)
        {
            return;
        }
        Debug.Log("-------------------------------3---------");

        int randomIndex = Random.Range(0, foodIcons.Count);

        while (unlocked.Contains(randomIndex))
        {
            randomIndex = Random.Range(0, foodIcons.Count);
        }
        Debug.Log("---------------------4-------------------");

        unlocked.Add(randomIndex);
    }

    public bool HasUnlockedThree()
    {
        return unlocked.Count >= 3;
    }

    public List<int> GetUnlockedIndexes()
    {
        return unlocked;
    }
}