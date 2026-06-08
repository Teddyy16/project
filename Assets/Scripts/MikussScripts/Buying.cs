using UnityEngine;
using TMPro;

public class Buying : MonoBehaviour
{
    public int Coin;
    public int Apple;
    public int Avocado;
    public int Cheese;
    public int Milk;
    public int Carrot;
    public int Meat;
    public int Bread;
    public int Soda;

    public TMP_Text Coin_Text;
    public TMP_Text Avocado_Text;
    public TMP_Text Cheese_Text;
    public TMP_Text Milk_Text;
    public TMP_Text Carrot_Text;
    public TMP_Text Meat_Text;
    public TMP_Text Bread_Text;
    public TMP_Text Apple_Text;
    public TMP_Text Soda_Text;

    void Start()
    {
        LoadValues();
        UpdateTexts();
    }

    private void LoadValues()
    {
        Coin = PlayerPrefs.GetInt("Coin", 0);

        Apple = PlayerPrefs.GetInt("Apple", 0);
        Avocado = PlayerPrefs.GetInt("Avocado", 0);
        Cheese = PlayerPrefs.GetInt("Cheese", 0);
        Milk = PlayerPrefs.GetInt("Milk", 0);
        Carrot = PlayerPrefs.GetInt("Carrot", 0);
        Meat = PlayerPrefs.GetInt("Meat", 0);
        Bread = PlayerPrefs.GetInt("Bread", 0);
        Soda = PlayerPrefs.GetInt("Soda", 0);
    }

    public void BuyApple()
    {
        BuyFood("Apple");
    }

    public void BuyAvocado()
    {
        BuyFood("Avocado");
    }

    public void BuyCheese()
    {
        BuyFood("Cheese");
    }

    public void BuyMilk()
    {
        BuyFood("Milk");
    }

    public void BuyCarrot()
    {
        BuyFood("Carrot");
    }

    public void BuyMeat()
    {
        BuyFood("Meat");
    }

    public void BuyBread()
    {
        BuyFood("Bread");
    }

    public void BuySoda()
    {
        BuyFood("Soda");
    }

    private void BuyFood(string foodName)
    {
        LoadValues();

        Debug.Log("BUY " + foodName + " BUTTON PRESSED");
        Debug.Log("Coins before buying: " + Coin);

        if (Coin < 10)
        {
            Debug.Log("Not enough coins.");
            return;
        }

        Coin -= 10;

        if (foodName == "Apple")
        {
            Apple += 1;
        }
        else if (foodName == "Avocado")
        {
            Avocado += 1;
        }
        else if (foodName == "Cheese")
        {
            Cheese += 1;
        }
        else if (foodName == "Milk")
        {
            Milk += 1;
        }
        else if (foodName == "Carrot")
        {
            Carrot += 1;
        }
        else if (foodName == "Meat")
        {
            Meat += 1;
        }
        else if (foodName == "Bread")
        {
            Bread += 1;
        }
        else if (foodName == "Soda")
        {
            Soda += 1;
        }

        SaveValues();

        Debug.Log(foodName + " bought.");
        Debug.Log("Saved " + foodName + " count: " + PlayerPrefs.GetInt(foodName, 0));
        Debug.Log("Coins after buying: " + Coin);

        UpdateTexts();
    }

    private void SaveValues()
    {
        PlayerPrefs.SetInt("Coin", Coin);

        PlayerPrefs.SetInt("Apple", Apple);
        PlayerPrefs.SetInt("Avocado", Avocado);
        PlayerPrefs.SetInt("Cheese", Cheese);
        PlayerPrefs.SetInt("Milk", Milk);
        PlayerPrefs.SetInt("Carrot", Carrot);
        PlayerPrefs.SetInt("Meat", Meat);
        PlayerPrefs.SetInt("Bread", Bread);
        PlayerPrefs.SetInt("Soda", Soda);

        PlayerPrefs.Save();
    }

    public void DebugAddCoins()
    {
        LoadValues();

        Coin += 100;

        PlayerPrefs.SetInt("Coin", Coin);
        PlayerPrefs.Save();

        Debug.Log("Added coins. Current coins: " + Coin);

        UpdateTexts();
    }

    public void DebugClearSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Coin = 0;

        Apple = 0;
        Avocado = 0;
        Cheese = 0;
        Milk = 0;
        Carrot = 0;
        Meat = 0;
        Bread = 0;
        Soda = 0;

        Debug.Log("Save cleared.");

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (Coin_Text != null) Coin_Text.text = Coin.ToString();

        if (Apple_Text != null) Apple_Text.text = Apple.ToString();
        if (Avocado_Text != null) Avocado_Text.text = Avocado.ToString();
        if (Cheese_Text != null) Cheese_Text.text = Cheese.ToString();
        if (Milk_Text != null) Milk_Text.text = Milk.ToString();
        if (Carrot_Text != null) Carrot_Text.text = Carrot.ToString();
        if (Meat_Text != null) Meat_Text.text = Meat.ToString();
        if (Bread_Text != null) Bread_Text.text = Bread.ToString();
        if (Soda_Text != null) Soda_Text.text = Soda.ToString();
    }
}