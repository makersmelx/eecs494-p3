using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    //This is a temporary victory condition script.
    public GameObject winPanel;


    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        winPanel.SetActive(true);
        PlayerInputHandler.Instance.ExitGameMode();
    }
}