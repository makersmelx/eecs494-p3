using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMoveControl : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Camera
    // -------------------------------------------------------------------------
    [Header("Camera")] [Tooltip("Reference to the main camera used for the player")]
    public GameObject playerCamera;

    [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;


    // -------------------------------------------------------------------------
    // Audio
    // -------------------------------------------------------------------------
    [Header("Audio")] [SerializeField] PlayerAudio playerAudio;


    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------
    [Header("Movement")] [Tooltip("Max movement speed when grounded")]
    public float initialMaxSpeed = 10f;

    [Tooltip("How fast the player can change his speed")]
    public float speedAcceleration = 200;

    [Tooltip("How fast the player can change his speed")]
    public float speedDeceleration = 120;

    [Tooltip("Smallest ratio for a velocity to still be considered motion")]
    public float lowVelocityThreshold = 0.01f;

    [Tooltip("How much to correct for sliding")]
    public float counterMovement = 0.175f;


    // -------------------------------------------------------------------------
    // Jump
    // -------------------------------------------------------------------------
    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 120f;

    [Tooltip("the time after the player leaves the ground when player still can jump")]
    public float jumpForgivingTime = 2f;

    [Tooltip("the time when holding the space can jump higher")]
    public float maxJumpDuration = 0.1f;


    // -------------------------------------------------------------------------
    // Ground detection
    // -------------------------------------------------------------------------
    [Header("Check Grounded")] public ObstructionDetection groundDetection;


    // -------------------------------------------------------------------------
    // Component reference
    // -------------------------------------------------------------------------

    private PlayerInputHandler playerInputHandler;
    private Rigidbody rigidBody;
    public static PlayerMoveControl Instance;


    // -------------------------------------------------------------------------
    // Runtime values
    // -------------------------------------------------------------------------
    public float currentCameraAngleVertical = 0f;
    private float lastGroundedTime = 0f;
    private float lastJumpTime = 0f;
    private bool isJumping = false;


    [SerializeField] private float currentMaxSpeedThreshold = 1f;

    // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
    public bool isWin = false;

    public bool isWallRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleCameraMove();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleCharacterMove();
        HandleCharacterJump();
    }

    // -------------------------------------------------------------------------
    // API
    // -------------------------------------------------------------------------
    public void SetCurrentMaxSpeedThreshold(float value)
    {
        currentMaxSpeedThreshold = value;
    }

    public float CurrentMaxSpeed => initialMaxSpeed * currentMaxSpeedThreshold;

    public bool IsGrounded { get; private set; }

    // todo: if a slower camera movement for aiming or taking photo is needed, modify here
    // The coefficient of the camera speed, may be affected by aiming or other actions
    public float CameraCoefficient
    {
        get { return 1f; }
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
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
        bool oldIsGrounded = IsGrounded;
        IsGrounded = groundDetection.isObstructed;
        if (oldIsGrounded && !IsGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (IsGrounded)
        {
            isJumping = false;
        }
    }

    private void HandleCharacterMove()
    {
        float speedCoefficient = 1f;
        Vector3 globalMoveInput = transform.TransformVector(playerInputHandler.GetMoveInput());
        Vector3 targetVelocity = globalMoveInput * CurrentMaxSpeed * speedCoefficient * speedAcceleration;
        // todo: if there is a crouch, reduce the speed by a ratio
        // todo: if there is a slope, adjust the velocity

        // Clamp the horizontal speed
        {
            Vector3 upVelocity = Vector3.Dot(rigidBody.velocity, Vector3.up) * Vector3.up;
            Vector3 horizonVelocity = rigidBody.velocity - upVelocity;
            rigidBody.velocity = Vector3.ClampMagnitude(horizonVelocity, CurrentMaxSpeed) + upVelocity;
            rigidBody.AddForce(targetVelocity * Time.deltaTime);
        }

        // Adjust the speed to zero when there is no input and the character is not in the air
        {
            if (IsGrounded)
            {
                float forwardSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
                float rightSpeed = Vector3.Dot(rigidBody.velocity, transform.right);
                if (playerInputHandler.GetMoveInput().magnitude < 1)
                {
                    if (Math.Abs(rightSpeed) > lowVelocityThreshold)
                    {
                        rigidBody.AddForce(-rightSpeed * counterMovement * speedDeceleration
                                           * transform.right * Time.deltaTime);
                    }

                    if (Math.Abs(forwardSpeed) > lowVelocityThreshold)
                    {
                        rigidBody.AddForce(-forwardSpeed * counterMovement * speedDeceleration
                                           * transform.forward * Time.deltaTime);
                    }
                }
            }
        }
    }

    // Jump Function will not check the character's current state
    private void HandleCharacterJump()
    {
        if (IsGrounded || Time.time - lastGroundedTime <= jumpForgivingTime)
        {
            if (playerInputHandler.GetJumpInputIsHolding())
            {
                if (!isJumping)
                {
                    isJumping = true;
                    lastJumpTime = Time.time;
                }

                if (isJumping && Time.time - lastJumpTime <= maxJumpDuration)
                {
                    playerAudio.PlayJumpSound();
                    rigidBody.AddForce(jumpForce * Vector3.up * Time.deltaTime, ForceMode.Impulse);
                }
            }
        }
    }
}