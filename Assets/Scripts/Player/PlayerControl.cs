using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerControl : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Assignable Component References
    // -------------------------------------------------------------------------
    [Header("Component References")]
    [Tooltip("Reference to the player camera")]
    [SerializeField] Camera playerCamera;

    [Tooltip("Reference to the player orientation")]
    [SerializeField] Transform orientation;

    [Tooltip("Reference to the player's Rigidbody")]
    [SerializeField] Rigidbody rigidBody;

    // -------------------------------------------------------------------------
    // Camera
    // -------------------------------------------------------------------------
    [Tooltip("Rotation speed for moving the camera")]
    public float cameraMoveSpeed = 200f;


    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------
    [Header("Movement")]
    [Tooltip("Parameter to adjust how fast character accelerates")]
    public float moveAcceleration = 4500f;

    [Tooltip("Max movement speed when grounded")]
    public float maxSpeed = 2f;

    [Tooltip("Defines what is considered to be 'ground'")]
    public LayerMask groundLayer;

    [Tooltip("Maximum slope angle that player can walk over")]
    public float maxSlopeAngle = 45f;

    [Tooltip("Smallest value for a velocity to still be considered motion")]
    public float lowVelocityThreshold = 0.01f;

    [Tooltip("How much to correct for sliding")]
    public float counterMovement = 0.175f;


    // -------------------------------------------------------------------------
    // Jump
    // -------------------------------------------------------------------------
    [Header("Jumping")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 650f;

    [Tooltip("How long player must wait between jumps")]
    public float jumpCooldown = 0.25f;


    // -------------------------------------------------------------------------
    // Wall walk
    // -------------------------------------------------------------------------
    [Header("Wall Walk")] [Tooltip("Physic layers checked to consider the player is close to a wall")]
    public LayerMask wallCheckLayers = -1;

    [Tooltip(
        "(Ratio to the character's radius) the max forward distance from the transform position to detect the wall")]
    public float wallDetectionMaxDistanceForwardRatio = 0.5f;

    [Tooltip("(Ratio to the character's radius) the radius of the wall detection sphere")]
    public float wallDetectionSphereRadiusRatio = 0.6f;

    [Tooltip("Color of the wall detection sphere for debug")]
    public Color wallDetectionColor = Color.red;


    // -------------------------------------------------------------------------
    // Ledge climb
    // -------------------------------------------------------------------------
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

    [Tooltip("Color of the ledge detection sphere for debug")]
    public Color ledgeDetectionColor = Color.yellow;


    // -------------------------------------------------------------------------
    // Private Component references
    // -------------------------------------------------------------------------
    private PlayerInputHandler playerInputHandler;


    // -------------------------------------------------------------------------
    // Runtime state
    // -------------------------------------------------------------------------
    private bool jumpReady = true;
    private float xRotation;
    private float yRotation;
    private bool grounded;
    private bool cancellingGrounded;
    private Vector3 currentGroundNormal;

    // TODO: If a slower camera movement for aiming or taking photo is needed, modify here
    // The coefficient of the camera speed, may be affected by aiming or other actions
    private float CameraCoefficient { get { return 1f; } }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    // Run every frame, updates player state
    private void Update()
    {
        HandleCameraRotation();
    }

    // Run every frame (in sync with physics engine), updates player state
    private void FixedUpdate()
    {
        HandleCharacterMovement();
    }

    // Use mouse input to make player turn and look up and down
    private void HandleCameraRotation()
    {
        Vector2 mouseInput = playerInputHandler.GetMouseInput();
        Vector3 currentRotation = orientation.transform.localRotation.eulerAngles;
        float xNewRotation = currentRotation.y + mouseInput.x;

        // Update rotation internal state
        xRotation -= (mouseInput.y * cameraMoveSpeed * CameraCoefficient);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += (mouseInput.x * cameraMoveSpeed * CameraCoefficient);

        // Rotate camera (Look up/down)
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate player and camera (Turning left/right)
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void HandleCharacterMovement()
    {
        Vector2 moveInput = playerInputHandler.GetMoveInput();
        Vector2 relativeVelocity = VelocityRelativeToLook();

        // Jump
        if (jumpReady && playerInputHandler.JumpKeyPressed()) Jump();

        // Movement multipliers
        float movementMultiplier = grounded ? 1f : 0.5f;

        // Add forces to player
        rigidBody.AddForce(orientation.transform.forward * moveInput.y * moveAcceleration * Time.deltaTime * movementMultiplier);
        rigidBody.AddForce(orientation.transform.right * moveInput.x * moveAcceleration * Time.deltaTime * movementMultiplier);

        // Correct for sliding
        MovementSlidingAdjustment(moveInput.x, moveInput.y, relativeVelocity);

        // Clamp velocity
        LimitVelocity();
    }

    private void Jump()
    {
        Debug.Log("Jump() called");
        Debug.Log("grounded :" + grounded);
        Debug.Log("jumpReady:" + jumpReady);
        if (grounded && jumpReady)
        {
            jumpReady = false;

            // Add jump forces
            rigidBody.AddForce(Vector3.up * jumpForce);

            // If jumping while falling, reset y velocity.
            Vector3 vel = rigidBody.velocity;
            if (rigidBody.velocity.y < 0.5f)
                rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rigidBody.velocity.y > 0)
                rigidBody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        jumpReady = true;
    }

    private bool IsFloor(Vector3 v)
    {
        Debug.Log("IsFloor() called");
        float angle = Vector3.Angle(Vector3.up, v);
        Debug.Log("Returning:" + (angle < maxSlopeAngle));
        return angle < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        // Only check for ground layers
        int layer = other.gameObject.layer;
        if (groundLayer != layer) return;

        // Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 otherNormal = other.contacts[i].normal;
            Debug.Log("Colliding with:" + other.contacts[i].thisCollider.name);

            if (IsFloor(otherNormal))
            {
                grounded = true;
                cancellingGrounded = false;
                currentGroundNormal = otherNormal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        // Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }

    private Vector2 VelocityRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rigidBody.velocity.x, rigidBody.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float velocityMagnitude = rigidBody.velocity.magnitude;
        float xMagnitude = velocityMagnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        float yMagnitude = velocityMagnitude * Mathf.Cos(u * Mathf.Deg2Rad);

        return new Vector2(xMagnitude, yMagnitude);
    }

    private void MovementSlidingAdjustment(float xInput, float yInput, Vector3 velocity)
    {
        if (!grounded) return;

        // Correct for sliding when player not holding down keys
        if (Math.Abs(velocity.x) > lowVelocityThreshold && Math.Abs(xInput) < 0.05f)
        {
            rigidBody.AddForce(moveAcceleration * orientation.transform.right * Time.deltaTime * -velocity.x * counterMovement);
        }
        if (Math.Abs(velocity.y) > lowVelocityThreshold && Math.Abs(yInput) < 0.05f)
        {
            rigidBody.AddForce(moveAcceleration * orientation.transform.forward * Time.deltaTime * -velocity.y * counterMovement);
        }
    }

    private void LimitVelocity()
    {
        if (rigidBody.velocity.magnitude > maxSpeed)
        {
            float fallspeed = rigidBody.velocity.y;
            Vector3 correctedVelocity = rigidBody.velocity.normalized * maxSpeed;
            rigidBody.velocity = new Vector3(correctedVelocity.x, fallspeed, correctedVelocity.z);
        }
    }
}