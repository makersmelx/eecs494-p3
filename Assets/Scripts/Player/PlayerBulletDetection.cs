using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletDetection : MonoBehaviour
{
    [SerializeField] float bulletHitTimeReduction = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.BulletTag))
        {
            TimeManager.Instance.ReduceTime(bulletHitTimeReduction);
        }
    }
}
