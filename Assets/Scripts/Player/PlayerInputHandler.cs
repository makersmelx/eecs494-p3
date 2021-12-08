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

    public bool inGameMode = false;

    // -------------------------------------------------------------------------
    // Internal state
    // -------------------------------------------------------------------------
    private bool canProcessMouseInput = false;

    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
    private static PlayerInputHandler _instance;

    public static PlayerInputHandler Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    // Hides cursor and locks it to the center of the screen
    public void EnterGameMode()
    {
        inGameMode = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Returns cursor control to the player
    public void ExitGameMode()
    {
        inGameMode = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Return the normalized movement Vector3
    public Vector3 GetMoveInput()
    {
        if (inGameMode && CanProcessInput())
        {
            float horizontalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisHorizontal);
            float verticalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisVertical);

            Vector3 moveInput = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            return moveInput;
        }

        return Vector3.zero;
    }

    public float GetCameraInputHorizontal()
    {
        if (inGameMode && CanProcessInput())
        {
            return GetMouseInputByAxis(GameConstants.MouseAxisHorizontal);
        }

        return 0;
    }

    public float GetCameraInputVertical()
    {
        if (inGameMode && CanProcessInput())
        {
            return GetMouseInputByAxis(GameConstants.MouseAxisVertical);
        }

        return 0;
    }

    public bool GetJumpInputIsHolding()
    {
        if (inGameMode && CanProcessInput())
        {
            return Input.GetButton(GameConstants.ButtonJump);
        }

        return false;
    }

    public bool GetJumpKeyDown()
    {
        if (inGameMode && CanProcessInput())
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        return false;
    }

    public bool GetMouseRightButton()
    {
        return inGameMode && CanProcessInput() && Input.GetMouseButton(1);
    }

    public bool GetEscapeButtonDown()
    {
        if (inGameMode && CanProcessInput())
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        return false;
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void Update()
    {
        // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
        if (IsMouseOverGameWindow && !PlayerMoveControl.Instance.isWin)
        {
            canProcessMouseInput = true;
        }
        else
        {
            canProcessMouseInput = false;
        }
    }

    // todo: add condition that cannot control
    public bool CanProcessInput()
    {
        return !LevelManager.Instance.isDead;
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