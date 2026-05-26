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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Coin = PlayerPrefs.GetInt("Coin");
        Apple = PlayerPrefs.GetInt("Apple");
        Avocado = PlayerPrefs.GetInt("Avocado");
        Cheese = PlayerPrefs.GetInt("Cheese");
        Milk = PlayerPrefs.GetInt("Milk");
        Carrot = PlayerPrefs.GetInt("Carrot");
        Bread = PlayerPrefs.GetInt("Bread");
        Meat = PlayerPrefs.GetInt("Meat");
        Soda = PlayerPrefs.GetInt("Soda");

        //Coin = 100;
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

    public void BuyApple()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Apple += 1;
            Apple_Text.text = Apple.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Apple", Apple);
        }

        else
        {
            print("Not Enough Coins");
        }
    }

         public void BuyAvocado()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Avocado += 1;
            Avocado_Text.text = Avocado.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Avocado", Avocado);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuyCheese()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Cheese += 1;
            Cheese_Text.text = Cheese.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Cheese", Cheese);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuyMilk()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Milk += 1;
            Milk_Text.text = Milk.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Milk", Milk);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuyCarrot()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Carrot += 1;
            Carrot_Text.text = Carrot.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Carrot", Carrot);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuyMeat()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Meat += 1;
            Meat_Text.text = Meat.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Meat", Meat);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuyBread()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Bread += 1;
            Bread_Text.text = Bread.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Bread", Bread);
        }

        else
        {
            print("Not Enough Coins");
        }

    }

    public void BuySoda()
    {
        if (Coin >= 10)
        {
            Coin -= 10;
            Coin_Text.text = Coin.ToString();

            Soda += 1;
            Soda_Text.text = Soda.ToString();

            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Soda", Soda);
        }

        else
        {
            print("Not Enough Coins");
        }

    }


}
