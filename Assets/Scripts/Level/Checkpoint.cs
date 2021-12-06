using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private int activatedTrial = -1;

    private void OnTriggerEnter(Collider other)
    {
        if (activatedTrial == LevelManager.Instance.currentTrial)
            return;

        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.UpdateCheckpoint(transform.position, other.transform.rotation);
            activatedTrial = LevelManager.Instance.currentTrial;
        }
    }
}