using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldControl : MonoBehaviour
{
    [Tooltip("Factor that is applied to the player's move speed")]
    public float shieldMaxSpeedThreshold = 0.8f;

    [Tooltip("Time Bonus when the shield destroys a single bullet")]
    public float defendBonusTime = 10f;

    // Destroy bullets on hit
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Shield detected collision with " + other.tag);
        if (other.CompareTag(GameConstants.BulletTag))
        {
            TimeManager.Instance.AddTime(defendBonusTime);
            Destroy(other.gameObject);
        }
    }
}