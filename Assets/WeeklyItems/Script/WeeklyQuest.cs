using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "WeeklyQuest", menuName = "Scriptable Objects/WeeklyQuest")]
public class WeeklyQuest : ScriptableObject
{
    public int maxIcons=3;
    public List<Sprite> foodIcons;
private bool _initialized = false;
    public List<int> unlocked ;

   private void Awake()
{
    if (unlocked == null)
        unlocked = new List<int>();

    string saved = PlayerPrefs.GetString("unlocked", "");
    if (saved != "")
    {
        // deserialize
        unlocked = saved.Split(',')
                        .Select(int.Parse)
                        .ToList();
    }

    if (unlocked.Count == 0)
        AddRandomIndex();
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

        PlayerPrefs.SetString("unlocked", string.Join(",", unlocked));
    PlayerPrefs.Save();
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