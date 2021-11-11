using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkour : MonoBehaviour
{
    // ============================================= Animator =============================================
    public Animator cameraAnimator;

    // ============================================= Drag and Friction =============================================
    [Header("Drag and Friction")]
    public float dragOnGround = 5f;
    public float dragInAir = 0.2f;

    // ============================================= Detection =============================================
    // Detection Reference
    [Header("Detection")]
    public ObstructionDetection climbObjectDetection; //checks for climb object

    public ObstructionDetection
        climbObstructionDetection; //checks if theres somthing in front of the object e.g walls that will not allow the player to climb

    public ObstructionDetection vaultObjectDetection; //checks for vault object

    public ObstructionDetection
        vaultObstructionDetection; //checks if theres somthing in front of the object e.g walls that will not allow the player to vault


    public ObstructionDetection leftWallDetection;
    public ObstructionDetection rightWallDetection;

    // ============================================= Wall Run =============================================
    [Header("Wall Run")][Tooltip("The force that players moves up on the wall")]
    public float wallRunUpForce = 4f;
    [Tooltip("How fast the force changes")]
    public float wallRunUpForceChangeRate = 6f;
    public float wallJumpUpVelocity = 7f;
    public float wallJumpForwardVelocity = 7f;
    
    private bool isWallRunning;
    private bool isWallRunningLeft;
    private bool isWallRunningRight;
    private bool canWallRun = true;
    private float upForce;


    // ============================================= Parkour =============================================
    private bool isParkour;
    private float parkourProgress;
    private float currentParkourMoveTime;

    // ============================================= Vault =============================================

    // ============================================= Climb =============================================
    private bool canClimb;
    [Header("Climb")][Tooltip("how long the vault takes")]
    public float climbTime; 

    public Transform climbEndPoint;

    // ============================================= Reference =============================================
    private PlayerMoveControl playerMoveControl;
    private Rigidbody rigidbodyRef;

    // ============================================= Runtime =============================================
    private Vector3 recordedMoveDestination;
    private Vector3 recordedMoveStart;

    private void Start()
    {
        playerMoveControl = GetComponent<PlayerMoveControl>();
        rigidbodyRef = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SetDrag();
        Climb();

        PerFormParkour();
        WallRun();
    }

    private void SetDrag()
    {
        rigidbodyRef.drag = playerMoveControl.IsGrounded
            ? dragOnGround
            : dragInAir;
    }

    private void Climb()
    {
        if (climbObjectDetection.isObstructed
            && !climbObstructionDetection.isObstructed
            && !isWallRunning)
        {
            canClimb = true;
        }

        if (canClimb)
        {
            canClimb = false;
            rigidbodyRef.isKinematic = true;
            recordedMoveDestination = climbEndPoint.position;
            recordedMoveStart = transform.position;
            isParkour = true;
            currentParkourMoveTime = climbTime;
            cameraAnimator.CrossFade("Climb", 0.1f);
        }
    }

    private void PerFormParkour()
    {
        if (isParkour && parkourProgress < 1f)
        {
            parkourProgress += Time.deltaTime / currentParkourMoveTime;
            transform.position = Vector3.Lerp(recordedMoveStart, recordedMoveDestination, parkourProgress);

            if (parkourProgress >= 1f)
            {
                isParkour = false;
                parkourProgress = 0f;
                rigidbodyRef.isKinematic = false;
            }
        }
    }

    private void WallRun()
    {
        if (playerMoveControl.IsGrounded)
        {
            canWallRun = true;
        }

        if (!playerMoveControl.IsGrounded
            && !isParkour
            && canWallRun)
        {
            if (leftWallDetection.isObstructed)
            {
                isWallRunningLeft = true;
                canWallRun = false;
                upForce = wallRunUpForce;
            }

            if (rightWallDetection.isObstructed)
            {
                isWallRunningRight = true;
                canWallRun = false;
                upForce = wallRunUpForce;
                print("there");
            }
        }

        if (isWallRunningLeft
            && !leftWallDetection.isObstructed
            || PlayerInputHandler.Instance.GetMoveInput().z <= 0f
            || rigidbodyRef.velocity.magnitude < 1f
        )
        {
            isWallRunningLeft = false;
            isWallRunningRight = false;
            print("not here");
        }

        if (isWallRunningRight
            && !rightWallDetection.isObstructed
            || PlayerInputHandler.Instance.GetMoveInput().z <= 0f
            || rigidbodyRef.velocity.magnitude < 1f
        )
        {
            isWallRunningLeft = false;
            isWallRunningRight = false;
            print("not here");
        }

        isWallRunning = isWallRunningLeft || isWallRunningRight;
        playerMoveControl.isWallRunning = isWallRunningLeft || isWallRunningRight;
        
        cameraAnimator.SetBool("WallLeft", isWallRunningLeft);
        cameraAnimator.SetBool("WallRight", isWallRunningRight);

        if (isWallRunning)
        {
            rigidbodyRef.velocity = new Vector3(rigidbodyRef.velocity.x,
                upForce,
                rigidbodyRef.velocity.z);
            upForce -= wallRunUpForceChangeRate * Time.deltaTime;

            if (PlayerInputHandler.Instance.GetJumpInputIsHolding())
            {
                rigidbodyRef.velocity = transform.forward * wallJumpForwardVelocity + transform.up * wallJumpUpVelocity;
                isWallRunningLeft = false;
                isWallRunningRight = false;
            }

            if (playerMoveControl.IsGrounded)
            {
                isWallRunningLeft = false;
                isWallRunningRight = false;
            }
        }
    }
}