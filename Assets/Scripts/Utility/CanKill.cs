using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanKill : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        LevelManager.Instance.ResetAtCheckpoint();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        LevelManager.Instance.ResetAtCheckpoint();

    }
}
