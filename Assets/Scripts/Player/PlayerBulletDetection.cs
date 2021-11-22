using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletDetection : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.BulletTag))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TimeManager.Instance.ReduceTime(bullet.timeReductionOnHit);
            PlayerAudio.Instance.PlayPlayerHitByBulletSound();
        }
    }
}
