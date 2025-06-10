using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro

public class Passive_Income : MonoBehaviour
{
    public int coinsPerStock = 10;
    private float checkInterval = 10f; 
    private float nextCheckTime;

    [Header("UI References")]
    public TextMeshProUGUI coinCountText;

    void Start()
    {
        nextCheckTime = Time.time + checkInterval;
        Debug.Log("Passive Income script started. Next check in " + checkInterval + " seconds.");

        UpdateCoinDisplay();
    }

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            AwardPassiveIncome();
            nextCheckTime = Time.time + checkInterval;
            Debug.Log("Next passive income check scheduled for: " + nextCheckTime);
        }
    }

    void AwardPassiveIncome()
    {
        int currentStock = PlayerPrefs.GetInt("Stock", 0);

        if (currentStock > 0)
        {
            int coinsToAward = currentStock * coinsPerStock;
            int currentDollars = PlayerPrefs.GetInt("CountDollar", 0);

            PlayerPrefs.SetInt("CountDollar", currentDollars + coinsToAward);
            PlayerPrefs.Save();
            Debug.Log("Awarded " + coinsToAward + " coins! Total dollars: " + PlayerPrefs.GetInt("CountDollar"));

            UpdateCoinDisplay();
        }
        else
        {
            Debug.Log("No stocks to generate passive income.");
        }
    }

    void UpdateCoinDisplay()
    {
        if (coinCountText != null) 
        {
            int currentDollars = PlayerPrefs.GetInt("CountDollar", 0);
            coinCountText.text = currentDollars.ToString();
            Debug.Log("Coin display updated to: " + currentDollars);
        }
        else
        {
            Debug.LogWarning("CoinCountText is not assigned in the Inspector! Cannot update UI.");
        }
    }
}