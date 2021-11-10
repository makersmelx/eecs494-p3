using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMoveControl : MonoBehaviour
{
    // ============================================= General =============================================

    // ============================================= Camera =============================================
    [Header("Camera")] [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;

    [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;


    // ============================================= Movement =============================================
    [Header("Movement")] [Tooltip("Max movement speed when grounded")]
    public float maxSpeedOnGround = 10f;

    [Tooltip("How fast the player can change his speed")]
    public float speedSharpnessOnGround = 200;

    [Tooltip("Smallest ratio for a velocity to still be considered motion")]
    public float lowVelocityThreshold = 0.01f;

    [Tooltip("How much to correct for sliding")]
    public float counterMovement = 0.175f;


    // ============================================= Jump =============================================
    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 120f;


    // ============================================= Check Grounded =============================================
    [Header("Check Grounded")] [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Tooltip("Distance from the bottom of the character controller capsule to check grounded")]
    public float groundCheckDistance = 1f;

    [Tooltip("Distance from the bottom of the character controller capsule to check on ground in the air")]
    public float groundCheckDistanceInAir = 0.07f;

    [Tooltip("The waiting time for the next ground detection since last jump")]
    public float checkGroundedCooldownTime = 0.2f;


    // ============================================= Component Reference ============================================= 
    private PlayerInputHandler playerInputHandler;
    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    public static PlayerMoveControl Instance;


    // todo: if a slower camera movement for aiming or taking photo is needed, modify here
    // The coefficient of the camera speed, may be affected by aiming or other actions
    public float CameraCoefficient
    {
        get { return 1f; }
    }

    public enum CharacterState
    {
        InAir = 0,
        DefaultGrounded = 1,
        WallRun = 2,
        OnLedge = 3,
    }


    // ============================================= Runtime Value ============================================= 
    // todo: make it not editable
    [Header("Runtime Value for Display")] public CharacterState currentState;

    private float currentCameraAngleVertical = 0f;
    private float lastJumpTime = 0f;

    private Vector3 currentGroundNormal;

    private Vector3 initialCameraOffset;
    private Vector3 cameraOffset = Vector3.zero;

    // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
    public bool isWin = false;

    private bool isJumping = false;

    // A count set to prevent multiple ledge climb check when the character is trying to leave the ledge
    private int ledgeJudgeCount = 0;

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
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentGroundNormal = Vector3.up;
        initialCameraOffset = playerCamera.transform.position - transform.position;
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

    // *******************************************************************************************
    // ******                                  Api                                          ******
    // *******************************************************************************************


    // *******************************************************************************************
    // ******                            Main Functions                                     ******
    // *******************************************************************************************
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
        float finalCheckDistance =
            currentState == CharacterState.InAir
                ? groundCheckDistanceInAir
                : groundCheckDistance;

        // reset
        Vector3 toGroundDirection = -currentGroundNormal;
        // only try to detect ground if it's been a short amount of time since last jump;
        if (Time.time >= lastJumpTime + checkGroundedCooldownTime)
        {
            Vector3 origin = transform.position + toGroundDirection * capsuleCollider.height / 2;
            bool isGrounded = Physics.Raycast(
                origin,
                toGroundDirection,
                out RaycastHit hit,
                finalCheckDistance
            );
            Debug.DrawLine(origin, origin + toGroundDirection * finalCheckDistance, Color.red);
            // True if the user is on the ground or on a wall
            if (isGrounded)
            {
                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, toGroundDirection * -1) > 0f)
                {
                    if (currentState == CharacterState.InAir)
                    {
                        currentState = CharacterState.DefaultGrounded;
                    }
                }
            }
            else
            {
                currentState = CharacterState.InAir;
            }
        }
    }

    private void HandleCharacterMove()
    {
        float speedCoefficient = 1f;
        Vector3 globalMoveInput = transform.TransformVector(playerInputHandler.GetMoveInput());
        Vector3 targetVelocity = globalMoveInput * maxSpeedOnGround * speedCoefficient * speedSharpnessOnGround;
        // todo: if there is a crouch, reduce the speed by a ratio
        // todo: if there is a slope, adjust the velocity

        // Clamp the horizontal speed
        {
            Vector3 upVelocity = Vector3.Dot(rigidBody.velocity, transform.up) * currentGroundNormal;
            Vector3 horizonVelocity = rigidBody.velocity - upVelocity;
            rigidBody.velocity = Vector3.ClampMagnitude(horizonVelocity, maxSpeedOnGround) + upVelocity;
            rigidBody.AddForce(targetVelocity * Time.deltaTime);
        }

        // Adjust the speed to zero when there is no input and the character is not in the air
        {
            if (currentState == CharacterState.DefaultGrounded)
            {
                float forwardSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
                float rightSpeed = Vector3.Dot(rigidBody.velocity, transform.right);
                if (Math.Abs(rightSpeed) > lowVelocityThreshold && Math.Abs(playerInputHandler.GetMoveInput().x) < 1)
                {
                    rigidBody.AddForce(-rightSpeed * counterMovement * speedSharpnessOnGround
                                       * transform.right * Time.deltaTime);
                }

                if (Math.Abs(forwardSpeed) > lowVelocityThreshold && Math.Abs(playerInputHandler.GetMoveInput().z) < 1)
                {
                    rigidBody.AddForce(-forwardSpeed * counterMovement * speedSharpnessOnGround
                                       * transform.forward * Time.deltaTime);
                }
            }
        }
    }

    // Jump Function will not check the character's current state
    private void HandleCharacterJump()
    {
        if (currentState != CharacterState.InAir)
        {
            if (!isJumping && playerInputHandler.GetJumpInputIsHolding())
            {
                rigidBody.AddForce(jumpForce * currentGroundNormal);
                currentState = CharacterState.InAir;
                currentGroundNormal = Vector3.up;
                lastJumpTime = Time.time;
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }
        }
    }
}