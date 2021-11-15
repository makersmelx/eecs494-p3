using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Component Reference
    // -------------------------------------------------------------------------
    [SerializeField] ShieldControl shield;
    [SerializeField] PlayerAudio playerAudio;


    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private bool isActive = false;


    // -------------------------------------------------------------------------
    // Update methods
    // -------------------------------------------------------------------------
    private void Start()
    {
        shield.gameObject.SetActive(false);
    }

    void Update()
    {
        SetActive();
        ChangePlayerMaxSpeed();
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void SetActive()
    {
        bool buttonPressed = PlayerInputHandler.Instance.GetMouseRightButton();

        // Activate
        if (!isActive && buttonPressed)
        {
            shield.gameObject.SetActive(true);
            playerAudio.PlayShieldActiveSound();
            isActive = true;
        }

        // Deactivate
        if (isActive && !buttonPressed)
        {
            shield.gameObject.SetActive(false);
            playerAudio.PlayShieldDeactiveSound();
            isActive = false;
        }
    }

    private void ChangePlayerMaxSpeed()
    {
        PlayerMoveControl.Instance.SetCurrentMaxSpeedThreshold(
            shield.gameObject.activeInHierarchy
                ? shield.shieldMaxSpeedThreshold
                : 1f);
    }
}