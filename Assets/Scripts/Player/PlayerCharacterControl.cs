using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler), typeof(CharacterController))]
public class PlayerCharacterControl : MonoBehaviour
{
    [Header("References")] [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;

    [Header("Movement")] [Tooltip("Max movement speed when grounded")]
    public float maxSpeedOnGround = 10f;

    [Tooltip("How fast the player can change his speed")]
    public float speedSharpnessOnGround = 15;

    [Header("General")] [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;

    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    [Header("Check Grounded")] [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Header("Check Grounded")]
    [Tooltip("Distance from the bottom of the character controller capsule to check grounded")]
    public float groundCheckDistance = 1f;

    [Header("Check Grounded")]
    [Tooltip("Distance from the bottom of the character controller capsule to check on ground in the air")]
    public float groundCheckDistanceInAir = 0.07f;

    [Header("Check Grounded")] [Tooltip("The waiting time for the next ground detection since last jump")]
    public float checkGroundedCooldownTime = 0.2f;

    [Header("Camera")] [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;


    public enum CharacterState
    {
        InAir = 0,
        DefaultGrounded = 1,
        WallWalk = 2,
    }

    // todo: make it not editable
    public CharacterState currentState;

    public Vector3 CharacterVelocity { get; set; }

    /*
     * The coefficient of the camera speed, may be affected by aiming or other actions
     * todo: if aiming is needed, modify here
     */
    public float CameraCoefficient
    {
        get { return 1f; }
    }

    /*
     * Component Reference
     */
    private PlayerInputHandler playerInputHandler;
    private CharacterController characterController;

    /*
     * Runtime Value
     */
    private float currentCameraAngleVertical = 0f;
    private float lastJumpTime = 0f;
    private Vector3 currentGroundNormal;

    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleCameraMove();
        CheckGrounded();
        HandleCharacterMove();
    }

    private void HandleCameraMove()
    {
        // horizontal character rotation
        {
            transform.Rotate(new Vector3(
                0f,
                playerInputHandler.GetCameraInputHorizontal() * cameraMoveSpeed * CameraCoefficient,
                0f
            ), Space.Self);
        }

        // Vertical camera rotation
        {
            currentCameraAngleVertical -=
                playerInputHandler.GetCameraInputVertical() * cameraMoveSpeed * CameraCoefficient;
            currentCameraAngleVertical = Mathf.Clamp(currentCameraAngleVertical, -89f, 89f);
            playerCamera.transform.localEulerAngles = new Vector3(currentCameraAngleVertical, 0, 0);
        }
    }

    private void CheckGrounded()
    {
        // Skip ground check when the character is walking on the wall
        if (currentState == CharacterState.WallWalk)
        {
            return;
        }

        float finalCheckDistance =
            currentState == CharacterState.DefaultGrounded
                ? groundCheckDistance + characterController.skinWidth
                : groundCheckDistanceInAir;

        // reset
        currentGroundNormal = Vector3.up;

        Vector3 toGroundDirection = Vector3.down;
        // only try to detect ground if it's been a short amount of time since last jump;
        if (Time.time >= lastJumpTime + checkGroundedCooldownTime)
        {
            Vector3 origin = transform.position + toGroundDirection * characterController.height / 2;
            if (Physics.Raycast(
                origin,
                toGroundDirection,
                out RaycastHit hit,
                finalCheckDistance
            ))
            {
                currentGroundNormal = hit.normal;
                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, toGroundDirection * -1) > 0f)
                {
                    currentState = CharacterState.DefaultGrounded;
                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(toGroundDirection * hit.distance);
                    }
                }
            }
        }
    }

    private void HandleCharacterMove()
    {
        float speedCoefficient = 1f;
        Vector3 globalMoveInput = transform.TransformVector(playerInputHandler.GetMoveInput());
        if (currentState == CharacterState.DefaultGrounded
            || currentState == CharacterState.WallWalk)
        {
            Vector3 targetVelocity = globalMoveInput * maxSpeedOnGround * speedCoefficient;
            // todo: if there is  crouch, reduce the speed by a ratio
            // todo: if there is a slope, adjust the velocity
            CharacterVelocity =
                Vector3.Lerp(CharacterVelocity, targetVelocity, speedSharpnessOnGround * Time.deltaTime);
            // Jumping
            if (playerInputHandler.GetJumpInputIsHolding())
            {
                // todo: if we need to clear the up vector of the velocity
                CharacterVelocity += transform.up * jumpForce;
                currentState = CharacterState.InAir;
                currentGroundNormal = Vector3.up;
                lastJumpTime = Time.time;
            }
        }
        else if (currentState == CharacterState.InAir)
        {
            // Only apply the gravity to get the character down when he is in the air
            Vector3 targetVelocity = globalMoveInput * maxSpeedOnGround * speedCoefficient;
            CharacterVelocity =
                Vector3.Lerp(CharacterVelocity, targetVelocity, speedSharpnessOnGround * Time.deltaTime);
            CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
        
        characterController.Move(CharacterVelocity * Time.deltaTime);
    }
}