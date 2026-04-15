using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coins = 0;
    public TMP_Text coinText; // Drag the TMP_Text here you want to change

    void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }
}