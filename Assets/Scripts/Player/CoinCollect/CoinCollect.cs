using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoinCollect : MonoBehaviour
{
    // Start is called before the first frame update
    public int coinsBeforeBonus = 3;
    public float bonusTime = 14f;
    public TextMeshProUGUI coinUI;
    public int totalCoins;
    public int coinBonusCount;

    private void Update()
    {
        coinUI.text = "Coin: " + totalCoins.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coinBonusCount += 1;
            totalCoins += 1;
            if (coinBonusCount == coinsBeforeBonus)
            {
                TimeManager.Instance.AddTime(bonusTime);
                coinBonusCount = 0;
            }
        }
    }
}