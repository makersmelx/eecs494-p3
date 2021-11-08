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

    // Store input state
    private Vector2 mouseInput;
    private Vector2 movementInput;
    private bool jumping;
    private bool crouching;
    private bool canProcessMouseInput = false;

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------

    // Arrow key / WASD Input
    public Vector2 GetMoveInput() { return movementInput.normalized; }

    // Horizontal mouse input
    public Vector2 GetMouseInput() { return mouseInput; }

    // Jump input
    public bool JumpKeyPressed() { return jumping; }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void Update()
    {
        if (!IsMouseOverGameWindow())
        {
            canProcessMouseInput = false;
            return;
        }

        // Collect user input each frame
        canProcessMouseInput = true;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateUserInput();
    }

    private void UpdateUserInput()
    {
        // Update movement x, y values
        float horizontalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisHorizontal);
        float verticalInput = Input.GetAxisRaw(GameConstants.KeyboardAxisVertical);
        movementInput = new Vector2(horizontalInput, verticalInput);

        // Update jump, crouch input
        jumping = Input.GetButton(GameConstants.ButtonJump);
        crouching = Input.GetKey(KeyCode.LeftShift);

        // Update mouse input
        float mouseX = Input.GetAxisRaw(GameConstants.MouseAxisHorizontal);
        float mouseY = Input.GetAxisRaw(GameConstants.MouseAxisVertical);
        mouseX *= cameraSensitivity * 0.01f;
        mouseY *= cameraSensitivity * 0.01f;

#if UNITY_WEBGL
            mouseX *= webGLCameraSensitivityMultiplier;
            mouseY *= webGLCameraSensitivityMultiplier;
#endif
        mouseInput = new Vector2(mouseX, mouseY);
    }

    private bool IsMouseOverGameWindow()
    {
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        return !(0 > xPos || 0 > yPos || Screen.width < xPos || Screen.height < yPos);
    }
}
