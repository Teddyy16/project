using UnityEngine;
using TMPro;

public class FridgeInventoryUI : MonoBehaviour
{
    public TMP_Text fridgeInventoryText;

    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        string text = "Fridge:\n";

        int apple = PlayerPrefs.GetInt("Apple", 0);
        int avocado = PlayerPrefs.GetInt("Avocado", 0);
        int cheese = PlayerPrefs.GetInt("Cheese", 0);
        int milk = PlayerPrefs.GetInt("Milk", 0);
        int carrot = PlayerPrefs.GetInt("Carrot", 0);
        int meat = PlayerPrefs.GetInt("Meat", 0);
        int bread = PlayerPrefs.GetInt("Bread", 0);
        int soda = PlayerPrefs.GetInt("Soda", 0);

        if (apple > 0) text += "Apple: x" + apple + "\n";
        if (avocado > 0) text += "Avocado: x" + avocado + "\n";
        if (cheese > 0) text += "Cheese: x" + cheese + "\n";
        if (milk > 0) text += "Milk: x" + milk + "\n";
        if (carrot > 0) text += "Carrot: x" + carrot + "\n";
        if (meat > 0) text += "Meat: x" + meat + "\n";
        if (bread > 0) text += "Bread: x" + bread + "\n";
        if (soda > 0) text += "Soda: x" + soda + "\n";

        if (text == "Fridge:\n")
        {
            text += "Empty";
        }

        if (fridgeInventoryText != null)
        {
            fridgeInventoryText.text = text;
        }
    }
}
