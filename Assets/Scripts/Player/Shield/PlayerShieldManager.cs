using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Component Reference
    // -------------------------------------------------------------------------
    [SerializeField] ShieldControl shield;
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] Slider shieldSlider;
    [SerializeField] CanvasGroup shieldCanvas;

    public float powerCapacity = 100f;
    public float powerDecreaseRate = 20f;
    public float powerRestoreRate = 15f;


    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private bool isActive = false;
    private float currentPower;

    // -------------------------------------------------------------------------
    // Update methods
    // -------------------------------------------------------------------------
    private void Start()
    {
        shield.gameObject.SetActive(false);
        currentPower = powerCapacity;

        if (shieldCanvas == null) Debug.LogError("Assign the shield canvas object");
        if (shieldSlider == null) Debug.LogError("Assign the shield slider object");

        shieldCanvas.alpha = 0.2f;
    }

    void Update()
    {
        SetActive();
        HandlePower();
        if (shieldSlider != null)
        {
            shieldSlider.value = currentPower / powerCapacity;
        }
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void SetActive()
    {
        bool buttonPressed = PlayerInputHandler.Instance.GetMouseRightButton();

        // Activate
        if (!isActive && buttonPressed && currentPower > 0f)
        {
            ActivateShield();
            shieldCanvas.alpha = 1f;
        }

        // Deactivate
        if (isActive && !buttonPressed)
        {
            DeactivateShield();
            shieldCanvas.alpha = .2f;
        }
    }

    private void ActivateShield()
    {
        shield.gameObject.SetActive(true);
        playerAudio.PlayShieldActiveSound();
        isActive = true;
    }

    private void DeactivateShield()
    {
        shield.gameObject.SetActive(false);
        playerAudio.PlayShieldDeactiveSound();
        isActive = false;
    }

    private void HandlePower()
    {
        if (isActive)
        {
            currentPower -= powerDecreaseRate * Time.deltaTime;
            if (currentPower <= 0.001f)
            {
                currentPower = 0f;
                DeactivateShield();
            }
        }
        else
        {
            if (currentPower < powerCapacity)
            {
                currentPower += powerRestoreRate * Time.deltaTime;
                currentPower = Math.Min(currentPower, powerCapacity);
            }
        }
    }
}