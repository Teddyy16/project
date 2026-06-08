using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class AlbumMenuControleron : MonoBehaviour
{
    public Button RabbitButton;
    public Button AxelotlButton;

    public AnimalData animalData;
        public void UpdateUI()
    { animalData.Refresh();
       RabbitButton.interactable = animalData.isRabbitUnlocked;
        if(animalData.isRabbitUnlocked)
            {
                RabbitButton.image.sprite=animalData.RabbitIcon;
                RabbitButton.interactable=!animalData.isRabbitEqip;
            }  
           AxelotlButton.interactable=animalData.isRabbitEqip;
        

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
