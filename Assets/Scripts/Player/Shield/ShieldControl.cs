using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldControl : MonoBehaviour
{
    [Tooltip("Factor that is applied to the player's move speed")]
    public float shieldMaxSpeedThreshold = 0.8f;

    // Destroy bullets on hit
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Shield detected collision with " + other.tag);
        if (other.CompareTag(GameConstants.BulletTag))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            // TimeManager.Instance.AddTime(bullet.timeIncreaseOnHit);
            Destroy(other.gameObject);
            PlayerAudio.Instance.PlayPlayerShieldAbsorbBulletSound();
        }
    }
}