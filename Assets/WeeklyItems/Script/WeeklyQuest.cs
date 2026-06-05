using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyQuest", menuName = "Scriptable Objects/WeeklyQuest")]
public class WeeklyQuest : ScriptableObject
{
    public List<Texture2D> foodIcons;
    private List<int> unlocked;
    void OnEnable()
    {
        unlocked= new();
        unlocked.Add(Random.Range(0,foodIcons.length));

    }
        

}
