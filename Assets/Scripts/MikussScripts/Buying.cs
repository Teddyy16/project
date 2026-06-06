using UnityEngine;
using UnityEngine.UI;

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

    public Text Coin_Text;
    public Text Avocado_Text;
    public Text Cheese_Text;
    public Text Milk_Text;
    public Text Carrot_Text;
    public Text Meat_Text;
    public Text Bread_Text;
    public Text Apple_Text;
    public Text Soda_Text;

    void Start()
    {
        Coin = PlayerPrefs.GetInt("Coin", 0);
        Apple = PlayerPrefs.GetInt("Apple", 0);
        Avocado = PlayerPrefs.GetInt("Avocado", 0);
        Cheese = PlayerPrefs.GetInt("Cheese", 0);
        Milk = PlayerPrefs.GetInt("Milk", 0);
        Carrot = PlayerPrefs.GetInt("Carrot", 0);
        Bread = PlayerPrefs.GetInt("Bread", 0);
        Meat = PlayerPrefs.GetInt("Meat", 0);
        Soda = PlayerPrefs.GetInt("Soda", 0);

        UpdateTexts();
    }

    public void BuyApple()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Apple += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Apple", Apple);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyAvocado()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Avocado += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Avocado", Avocado);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyCheese()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Cheese += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Cheese", Cheese);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyMilk()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Milk += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Milk", Milk);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyCarrot()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Carrot += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Carrot", Carrot);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyMeat()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Meat += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Meat", Meat);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuyBread()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Bread += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Bread", Bread);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    public void BuySoda()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Soda += 1;

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Soda", Soda);
            PlayerPrefs.Save();

            UpdateTexts();
        }
    }

    private void UpdateTexts()
    {
        Coin_Text.text = Coin.ToString();
        Apple_Text.text = Apple.ToString();
        Avocado_Text.text = Avocado.ToString();
        Cheese_Text.text = Cheese.ToString();
        Milk_Text.text = Milk.ToString();
        Carrot_Text.text = Carrot.ToString();
        Bread_Text.text = Bread.ToString();
        Meat_Text.text = Meat.ToString();
        Soda_Text.text = Soda.ToString();
    }
}