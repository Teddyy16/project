using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    private const string CoinsKey = "PlayerCoins";

    public TMP_Text coinText;
    public int coins;

    void Start()
    {
        LoadCoins();
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

    void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.Save();
    }

    void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(CoinsKey, 0);
    }
}
