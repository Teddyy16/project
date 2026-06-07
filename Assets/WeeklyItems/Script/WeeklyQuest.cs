using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyQuest", menuName = "Scriptable Objects/WeeklyQuest")]
public class WeeklyQuest : ScriptableObject
{
    public List<Texture2D> foodIcons;

    [SerializeField] private List<int> unlocked = new List<int>();

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
    {
        if (foodIcons == null || foodIcons.Count == 0)
        {
            Debug.LogWarning("WeeklyQuest has no food icons.");
            return;
        }

        if (unlocked.Count >= foodIcons.Count)
        {
            return;
        }

        int randomIndex = Random.Range(0, foodIcons.Count);

        while (unlocked.Contains(randomIndex))
        {
            randomIndex = Random.Range(0, foodIcons.Count);
        }

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