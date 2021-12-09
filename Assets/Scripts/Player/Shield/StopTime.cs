using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTime : MonoBehaviour
{
    public float timeScale = 0.1f;
    public float powerConsume = 60f;
    public float minTimeStopCapacity = 60f;
    public PlayerShieldManager playerShieldManager;
    private bool needRestore = false;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInputHandler.Instance.GetMouseLeftButton())
        {
            if (needRestore)
            {
                if (playerShieldManager.currentPower > minTimeStopCapacity)
                {
                    needRestore = false;
                }
                else
                {
                    CeaseTimeStop();
                    return;
                }
            }

            StartTimeStop();
        }
        else
        {
            CeaseTimeStop();
        }
    }

    private void StartTimeStop()
    {
        LevelManager.Instance.timeScale = timeScale;
        playerShieldManager.currentPower -= powerConsume * Time.deltaTime;
        if (playerShieldManager.currentPower < 0.001f)
        {
            playerShieldManager.currentPower = 0f;
            needRestore = true;
        }

        playerShieldManager.shieldCanvas.alpha = 1f;
    }

    private void CeaseTimeStop()
    {
        LevelManager.Instance.timeScale = 1f;
        playerShieldManager.shieldCanvas.alpha = playerShieldManager.isActive ? 1f : 0.2f;
    }
}