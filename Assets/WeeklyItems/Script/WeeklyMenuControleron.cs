using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeeklyMenuControleron : MonoBehaviour
{ 
    public List<Image> itemicons; 
    public WeeklyQuest weeklyQuest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        int i =0;
        foreach(int index in weeklyQuest.unlocked)
        
        { 
            itemicons[i].sprite=weeklyQuest.foodIcons[index] ;
            i++;

        }
    }
}
