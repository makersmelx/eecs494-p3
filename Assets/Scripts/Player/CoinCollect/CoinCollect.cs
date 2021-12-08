using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoinCollect : MonoBehaviour
{
    // Start is called before the first frame update
    public int totalCoins;
    public float speedBoostThreshold;
    public float speedBoostDuration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            totalCoins += 1;
            PlayerMoveControl.Instance.BoostMaxSpeed(speedBoostThreshold, speedBoostDuration);
        }
    }
}