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

    private void Update()
    {
        if (IsMouseOverGameWindow)
        {
            canProcessMouseInput = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            canProcessMouseInput = false;
        }
    }

    // todo: add condition that cannot control
    public bool CanProcessInput()
    {
        return true;
    }

    /*
     * return the normalized movement Vector3
     */
    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(
                Input.GetAxisRaw(GameConstants.KeyboardAxisNameHorizontal),
                0f,
                Input.GetAxisRaw(GameConstants.KeyboardAxisNameVertical)
            );

            move = Vector3.ClampMagnitude(move, 1);
            return move;
        }

        return Vector3.zero;
    }

    public float GetCameraInputHorizontal()
    {
        return GetMouseInputByAxis(GameConstants.MouseAxisNameHorizontal);
    }

    public float GetCameraInputVertical()
    {
        return GetMouseInputByAxis(GameConstants.MouseAxisNameVertical);
    }

    public bool GetJumpInputIsHolding()
    {
        return CanProcessInput() && Input.GetButton(GameConstants.ButtonNameJump);
    }


    private float GetMouseInputByAxis(string mouseInputName)
    {
        if (CanProcessInput() && canProcessMouseInput)
        {
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

        return 0f;
    }

    bool IsMouseOverGameWindow =>
        !(0 > Input.mousePosition.x
          || 0 > Input.mousePosition.y
          || Screen.width < Input.mousePosition.x
          || Screen.height < Input.mousePosition.y
            );
}
