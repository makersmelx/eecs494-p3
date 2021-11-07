using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float cameraSensitivity = 1f;

    [Tooltip("Additional sensitivity multiplier for WebGL on the base of APP build")]
    public float webGLCameraSensitivityMultiplier = 0.25f;

    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;

    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;

    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    private bool canProcessMouseInput = false;

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    // Arrow key / WASD Input
    public Vector3 GetMoveInput()
    {
        if (!CanProcessInput()) return Vector3.zero;

        // Get input  and save in a normalized Vector3
        float horizontalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisNameHorizontal);
        float verticalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisNameVertical);
        Vector3 movementInput = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        return movementInput;
    }

    // Horizontal mouse input
    public float GetCameraInputHorizontal()
    {
        return GetMouseInputByAxis(GameConstants.MouseAxisNameHorizontal);
    }

    // Vertical mouse input
    public float GetCameraInputVertical()
    {
        return GetMouseInputByAxis(GameConstants.MouseAxisNameVertical);
    }

    // Jump input
    public bool GetJumpInputIsHolding()
    {
        return CanProcessInput() && Input.GetButton(GameConstants.ButtonNameJump);
    }

    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void Update()
    {
        if (IsMouseOverGameWindow())
        {
            canProcessMouseInput = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            canProcessMouseInput = false;
        }
    }

    private float GetMouseInputByAxis(string mouseInputName)
    {
        if (!CanProcessInput()) return 0f;

        float mouseLook = Input.GetAxisRaw(mouseInputName);

        // todo: make sure of this
        if (invertYAxis)
        {
            mouseLook *= -1f;
        }

        mouseLook *= cameraSensitivity * 0.01f;

        #if UNITY_WEBGL
            mouseLook *= webGLCameraSensitivityMultiplier;
        #endif

        return mouseLook;
    }

    private bool IsMouseOverGameWindow()
    {
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        return !(0 > xPos || 0 > yPos || Screen.width < xPos || Screen.height < yPos);
    }

    // TODO: Add condition that cannot control
    private bool CanProcessInput()
    {
        return true;
    }
}
