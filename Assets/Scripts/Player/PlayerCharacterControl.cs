using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler), typeof(CharacterController))]
public class PlayerCharacterControl : MonoBehaviour
{
    // ============================================= General =============================================
    [Header("General")] [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;


    // ============================================= Camera =============================================
    [Header("Camera")] [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;

    [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;


    // ============================================= Movement =============================================
    [Header("Movement")] [Tooltip("Max movement speed when grounded")]
    public float maxSpeedOnGround = 10f;

    [Tooltip("How fast the player can change his speed")]
    public float speedSharpnessOnGround = 15;


    // ============================================= Jump =============================================
    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;


    // ============================================= Check Grounded =============================================
    [Header("Check Grounded")] [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Tooltip("Distance from the bottom of the character controller capsule to check grounded")]
    public float groundCheckDistance = 1f;

    [Tooltip("Distance from the bottom of the character controller capsule to check on ground in the air")]
    public float groundCheckDistanceInAir = 0.07f;

    [Tooltip("The waiting time for the next ground detection since last jump")]
    public float checkGroundedCooldownTime = 0.2f;


    // ============================================= Wall Walk =============================================
    [Header("Wall Walk")] [Tooltip("Physic layers checked to consider the player is close to a wall")]
    public LayerMask wallCheckLayers = -1;

    [Tooltip(
        "(Ratio to the character's radius) the max forward distance from the transform position to detect the wall")]
    public float wallDetectionMaxDistanceForwardRatio = 0.5f;

    [Tooltip("(Ratio to the character's radius) the radius of the wall detection sphere")]
    public float wallDetectionSphereRadiusRatio = 0.6f;

    [Tooltip("Color of the wall detection sphere for debug")]
    public Color wallDetectionColor = Color.red;

    private Vector3 WallDetectionCastOrigin => transform.position +
                                               transform.forward *
                                               (characterController.radius - WallDetectionSphereRadius);

    private float WallDetectionMaxDistance =>
        characterController.radius * wallDetectionMaxDistanceForwardRatio;

    private float WallDetectionSphereRadius => characterController.radius * wallDetectionSphereRadiusRatio;


    // ============================================= Ledge Climb =============================================
    [Header("Ledge Climb")] [Tooltip("Physic layers checked to consider the player is close to a ledge")]
    public LayerMask ledgeCheckLayers = -1;

    [Tooltip(
        "(Ratio to the character's height) the height of the origin of the detection compared to the transform position")]
    public float ledgeDetectionHeightRatio = 0.3f;

    [Tooltip(
        "(Ratio to the character's radius) the max forward distance from the origin position to detect the wall")]
    public float ledgeDetectionMaxDistanceForwardRatio = 0.5f;

    [Tooltip("(Ratio to the character's radius) the radius of the ledge detection sphere")]
    public float ledgeDetectionSphereRadiusRatio = 0.6f;

    [Tooltip(
        "(Ratio to the character's height) Describe how much the camera will move downward to see the ledge when climbing ledge")]
    public float ledgeCameraDownRatio = 0.4f;

    [Tooltip(
        "(Ratio to the character's radius) Describe how much the camera will move backward to see the ledge when climbing ledge")]
    public float ledgeCameraBackRatio = 0.5f;

    [Tooltip("Color of the ledge detection sphere for debug")]
    public Color ledgeDetectionColor = Color.yellow;

    private Vector3 LedgeDetectionCastOrigin =>
        transform.position
        + transform.up * ledgeDetectionHeightRatio * characterController.height
        + transform.forward * (-LedgeDetectionSphereRadius);

    private float LedgeDetectionMaxDistance =>
        characterController.radius * ledgeDetectionMaxDistanceForwardRatio;

    private float LedgeDetectionSphereRadius => characterController.radius * ledgeDetectionSphereRadiusRatio;


    // ============================================= Component Reference ============================================= 
    private PlayerInputHandler playerInputHandler;
    private CharacterController characterController;
    public static PlayerCharacterControl Instance;


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
        WallWalk = 2,
        OnLedge = 3,
    }


    // ============================================= Runtime Value ============================================= 
    // todo: make it not editable
    [Header("Runtime Value for Display")] public CharacterState currentState;

    // record the current velocity
    private Vector3 characterVelocity;
    private float currentCameraAngleVertical = 0f;
    private float lastJumpTime = 0f;

    private Vector3 currentGroundNormal;
    private Vector3 currentLedgeNormal;

    // This should not change during one jump and its falling down
    private Vector3 currentJumpNormal;

    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        characterController = GetComponent<CharacterController>();
        currentGroundNormal = Vector3.up;
    }

    private void Update()
    {
        HandleCameraMove();

        CheckGrounded();

        HandleCharacterMove();
        HandleCharacterJumpOntoWall();
        HandleCharacterClimbLedge();
    }

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
                : groundCheckDistance + characterController.skinWidth;

        // reset
        Vector3 toGroundDirection = -1f * currentGroundNormal;
        // only try to detect ground if it's been a short amount of time since last jump;
        if (Time.time >= lastJumpTime + checkGroundedCooldownTime)
        {
            // todo: when walking on the wall, the character now sticks half of the body inside the wall...So here is a tmp solution to make the detection work
            Vector3 origin = transform.position +
                             (currentState == CharacterState.WallWalk
                                 ? Vector3.zero
                                 : toGroundDirection * characterController.height / 2);

            // True if the user is on the ground or on a wall
            if (Physics.Raycast(
                origin,
                toGroundDirection,
                out RaycastHit hit,
                finalCheckDistance,
                groundCheckLayers,
                QueryTriggerInteraction.Ignore
            ))
            {
                currentGroundNormal = hit.normal;
                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, toGroundDirection * -1) > 0f)
                {
                    currentJumpNormal = currentGroundNormal;
                    if (currentState == CharacterState.InAir)
                    {
                        currentState = CharacterState.DefaultGrounded;
                    }

                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(toGroundDirection * hit.distance);
                    }
                }
            }
            // when the character reaches the end of a wall, he will fall
            else if (currentState == CharacterState.WallWalk)
            {
                currentState = CharacterState.InAir;
                Vector3 forwardHorizontal = GetForwardVectorClimbingWall(currentGroundNormal, Vector3.up);
                currentGroundNormal = Vector3.up;
                transform.rotation = Quaternion.LookRotation(forwardHorizontal, Vector3.up);
            }
            else if (currentState == CharacterState.OnLedge)
            {
                return;
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
        Vector3 targetVelocity = globalMoveInput * maxSpeedOnGround * speedCoefficient;
        // todo: if there is a crouch, reduce the speed by a ratio
        // todo: if there is a slope, adjust the velocity
        characterVelocity =
            Vector3.Lerp(characterVelocity, targetVelocity, speedSharpnessOnGround * Time.deltaTime);

        if (currentState == CharacterState.DefaultGrounded
            || currentState == CharacterState.WallWalk)
        {
            HandleCharacterJump();
        }
        else if (currentState == CharacterState.InAir)
        {
            HandleCharacterGravity();
        }
        else if (currentState == CharacterState.OnLedge)
        {
            HandleCharacterJump();
        }

        characterController.Move(characterVelocity * Time.deltaTime);
    }

    // Jump Function will not check the character's current state
    private void HandleCharacterJump()
    {
        if (playerInputHandler.GetJumpInputIsHolding())
        {
            // todo: if we need to clear the up vector of the velocity
            characterVelocity += currentJumpNormal * jumpForce;
            currentState = CharacterState.InAir;
            currentGroundNormal = Vector3.up;
            // todo: add coroutine to make it not so dizzy
            Vector3 forwardHorizontal = transform.forward;
            forwardHorizontal.y = 0;
            transform.rotation = Quaternion.LookRotation(forwardHorizontal, Vector3.up);
            lastJumpTime = Time.time;
        }
    }

    private void HandleCharacterGravity()
    {
        if (currentState == CharacterState.InAir)
        {
            // Only apply the gravity to get the character down when he is in the air
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
    }

    private void HandleCharacterJumpOntoWall()
    {
        if (currentState == CharacterState.InAir
            && Physics.SphereCast(
                WallDetectionCastOrigin,
                WallDetectionSphereRadius,
                transform.forward,
                out RaycastHit hit,
                WallDetectionMaxDistance,
                wallCheckLayers,
                QueryTriggerInteraction.Ignore))
        {
            currentState = CharacterState.WallWalk;

            // todo: add coroutine to make it not so dizzy
            // When climbing the wall, the character will rotate during which the camera will always look at the forward direction

            // A vector is a forward direction if it is a normal vector
            // and current ground normal, wall normal and this vector are intersectional with each other

            // This value is the forward direction that will not change when climbing to the wall
            Vector3 forwardHorizontal = GetForwardVectorClimbingWall(currentGroundNormal, hit.normal);

            transform.rotation = Quaternion.LookRotation(forwardHorizontal, hit.normal);
            currentGroundNormal = hit.normal;
            transform.position = hit.point + hit.normal * characterController.height;

            characterVelocity -= currentGroundNormal * jumpForce -
                                 currentGroundNormal * Vector3.Dot(characterVelocity, currentGroundNormal);
        }
    }

    private void HandleCharacterClimbLedge()
    {
        bool canClimbLedge = Physics.SphereCast(
            LedgeDetectionCastOrigin,
            LedgeDetectionSphereRadius,
            transform.forward,
            out RaycastHit hit,
            LedgeDetectionMaxDistance,
            ledgeCheckLayers,
            QueryTriggerInteraction.Ignore);

        if (currentState == CharacterState.InAir)
        {
            if (canClimbLedge)
            {
                currentState = CharacterState.OnLedge;
                currentLedgeNormal = hit.normal;
                characterVelocity = Vector3.zero;
                playerCamera.transform.position -= transform.up * ledgeCameraDownRatio * characterController.height
                                                   + transform.forward * ledgeCameraBackRatio *
                                                   characterController.radius;
                ;
            }
        }
        else if (currentState == CharacterState.OnLedge)
        {
            if (characterVelocity.magnitude > maxSpeedOnGround * 0.1 && !canClimbLedge)
            {
                currentState = CharacterState.InAir;
                playerCamera.transform.position += transform.up * ledgeCameraDownRatio * characterController.height
                                                   + transform.forward * ledgeCameraBackRatio *
                                                   characterController.radius;
            }
        }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = wallDetectionColor;
    //     Gizmos.DrawSphere(
    //         WallDetectionCastOrigin + transform.forward * WallDetectionMaxDistance,
    //         WallDetectionSphereRadius);
    //
    //     Gizmos.color = ledgeDetectionColor;
    //     Gizmos.DrawSphere(
    //         LedgeDetectionCastOrigin + transform.forward * LedgeDetectionMaxDistance,
    //         WallDetectionSphereRadius);
    // }

    private Vector3 GetForwardVectorClimbingWall(Vector3 currentNormal, Vector3 nextNormal)
    {
        Vector3 forwardHorizontal = Vector3.Cross(currentNormal, nextNormal);

        // If the player is facing back, reverse this value
        if (Vector3.Dot(forwardHorizontal, transform.forward) < 0)
        {
            forwardHorizontal *= -1;
        }

        return forwardHorizontal;
    }
}