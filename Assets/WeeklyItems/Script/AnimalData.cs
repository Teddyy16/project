using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
public class AnimalData : ScriptableObject
{
    public Sprite RabbitIcon ;
    public bool isRabbitUnlocked=false;
        public bool isRabbitEqip = false;
    
  
    public void EquipRabbit()
    {
        isRabbitEqip=true;
    }
    public void EquipAxelotlJelly()
    {
       isRabbitEqip=false;
    }
}
