using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCharacterControl : MonoBehaviour
{
    [Header("References")] [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;

    [Header("Movement")] [Tooltip("Max movement speed when grounded")]
    public float maxSpeedOnGround = 10f;

    [Tooltip("How fast the player can change his speed")]
    public float accelerationOnGround = 15;

    [Header("General")] [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;

    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    [Header("Camera")] [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;

    /*
     * The coefficient of the camera speed, may be affected by aiming or other actions
     * todo: if aiming is needed, modify here
     */
    public float CameraCoefficient
    {
        get { return 1f; }
    }

    private PlayerInputHandler playerInputHandler;


    private float currentCameraAngleVertical = 0f;

    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleCameraMove();
    }

    private void HandleCameraMove()
    {
        /*
         * horizontal character rotation
         */
        {
            transform.Rotate(new Vector3(
                0f,
                playerInputHandler.GetCameraInputHorizontal() * cameraMoveSpeed * CameraCoefficient,
                0f
            ), Space.Self);
        }

        /*
         * Vertical camera rotation
         */
        {
            currentCameraAngleVertical -=
                playerInputHandler.GetCameraInputVertical() * cameraMoveSpeed * CameraCoefficient;
            currentCameraAngleVertical = Mathf.Clamp(currentCameraAngleVertical, -89f, 89f);
            playerCamera.transform.localEulerAngles = new Vector3(currentCameraAngleVertical, 0, 0);
        }
    }
}