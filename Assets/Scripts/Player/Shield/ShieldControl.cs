using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldControl : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.BulletTag))
        {
            Destroy(other.gameObject);
            // todo: add time
        }
    }
}