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

    /// <summary>
    /// Camera Bobbing
    /// </summary>
    public GameObject playerCameraAnimator;

    public float bobFrequency = 5f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;
    [Range(0, 1)] public float headBobSmoothing = 0.1f;


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
    private PlayerParkour playerParkour;
    private Rigidbody rigidBody;
    public static PlayerMoveControl Instance;


    // -------------------------------------------------------------------------
    // Runtime values
    // -------------------------------------------------------------------------
    public float currentCameraAngleVertical = 0f;
    private float lastGroundedTime = 0f;
    private float lastJumpTime = 0f;
    private bool isJumping = false;
    private Coroutine speedBoostCoroutine;


    [SerializeField] private float currentMaxSpeedThreshold = 1f;

    // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
    public bool isWin = false;

    private float bobTimer;

    [SerializeField] private Vector3 transport;

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
        playerParkour = GetComponent<PlayerParkour>();
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
    public void BoostMaxSpeed(float value, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        StartCoroutine(IEnumerateSpeedBoost(value, duration));
    }

    IEnumerator IEnumerateSpeedBoost(float value, float duration)
    {
        currentMaxSpeedThreshold = value;
        yield return new WaitForSeconds(duration);
        currentMaxSpeedThreshold = 1f;
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

        // head bob
        {
            bool willBob = !playerParkour.isParkour
                           && !isJumping
                           && (IsGrounded || playerParkour.isWallRunning)
                           && rigidBody.velocity.magnitude > 1f;

            if (willBob)
            {
                bobTimer += Time.deltaTime;
            }
            else
            {
                bobTimer = 0;
            }

            Vector3 targetCameraPosition = playerCameraAnimator.transform.position + CalculateBobOffset(bobTimer);

            playerCamera.transform.position =
                Vector3.Lerp(playerCamera.transform.position, targetCameraPosition, headBobSmoothing);
            if ((playerCamera.transform.position - targetCameraPosition).magnitude <= 0.001f)
            {
                playerCamera.transform.position = targetCameraPosition;
            }
        }
    }

    private Vector3 CalculateBobOffset(float timer)
    {
        Vector3 offset = Vector3.zero;
        float cof = Mathf.Pow(currentMaxSpeedThreshold, 2);
        if (timer > 0)
        {
            float horizon = Mathf.Cos(timer * bobFrequency * cof) * bobHorizontalAmplitude * cof;
            float vertical = Mathf.Sin(timer * bobFrequency * 2 * cof) * bobVerticalAmplitude * cof;
            offset = playerCameraAnimator.transform.right * horizon + playerCameraAnimator.transform.up * vertical;
        }

        return offset;
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
        if ((IsGrounded || Time.time - lastGroundedTime <= jumpForgivingTime) && !playerParkour.isParkour)
        {
            if (playerInputHandler.GetJumpInputIsHolding())
            {
                if (!isJumping)
                {
                    isJumping = true;
                    lastJumpTime = Time.time;
                    playerAudio.PlayJumpSound();
                }

                if (isJumping && Time.time - lastJumpTime <= maxJumpDuration)
                {
                    rigidBody.AddForce(jumpForce * Vector3.up * Time.deltaTime, ForceMode.Impulse);
                }
            }
        }
    }
}