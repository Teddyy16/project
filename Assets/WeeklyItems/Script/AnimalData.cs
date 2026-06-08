using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
public class AnimalData : ScriptableObject
{
    public Sprite RabbitIcon;
    public bool isRabbitUnlocked = false;
    public bool isRabbitEqip = false;

    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh() {
         isRabbitUnlocked = PlayerPrefs.GetInt("isRabbitUnlocked", 0) == 1;
        isRabbitEqip = PlayerPrefs.GetInt("isRabbitEqip", 0) == 1;

    }

    public void EquipRabbit()
    {
        isRabbitEqip = true;
        PlayerPrefs.SetInt("isRabbitEqip", 1);
        PlayerPrefs.Save();
    }

    public void EquipAxelotlJelly()
    {
        isRabbitEqip = false;
        PlayerPrefs.SetInt("isRabbitEqip", 0);
        PlayerPrefs.Save();
    }

    public void UnlockRabbit()
    {
        isRabbitUnlocked = true;
        PlayerPrefs.SetInt("isRabbitUnlocked", 1);
        PlayerPrefs.Save();
    }
}