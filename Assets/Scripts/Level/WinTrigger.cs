using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    //This is a temporary victory condition script.
    public GameObject winPanel;

    private void OnTriggerEnter(Collider other)
    {
        winPanel.SetActive(true);
        PlayerInputHandler.Instance.ExitGameMode();
        AudioManager.Instance.PlayWinSound();
    }
}