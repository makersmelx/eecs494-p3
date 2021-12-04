using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerParkour : MonoBehaviour
{
    // ============================================= Animator =============================================
    public Animator cameraAnimator;

    // ============================================= Drag and Friction =============================================
    [Header("Drag and Friction")] public float dragOnGround = 5f;
    public float dragInAir = 0.2f;

    // ============================================= Detection =============================================
    // Detection Reference
    [Header("Detection")] public ObstructionDetection climbObjectDetection; //checks for climb object

    public ObstructionDetection
        climbObstructionDetection; //checks if theres somthing in front of the object e.g walls that will not allow the player to climb

    public ObstructionDetection vaultObjectDetection; //checks for vault object

    public ObstructionDetection
        vaultObstructionDetection; //checks if theres somthing in front of the object e.g walls that will not allow the player to vault


    public ObstructionDetection leftWallDetection;
    public ObstructionDetection rightWallDetection;

    // ============================================= Wall Run =============================================
    [Header("Wall Run")] [Tooltip("The force that players moves up on the wall")]
    public float wallRunUpForce = 4f;

    [Tooltip("How fast the force changes")]
    public float wallRunUpForceChangeRate = 6f;

    public float wallJumpHorizonForce = 500f;
    public float wallJumpUpForce = 10f;
    public float wallJumpMaxDuration = 1f;

    [Tooltip("How long it takes to rotate the camera")]
    public float cameraAnimationDuration = 0.25f;

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

    [Header("Climb")] [Tooltip("how long the vault takes")]
    public float climbTime;

    public Transform climbEndPoint;

    // ============================================= Reference =============================================
    private PlayerMoveControl playerMoveControl;
    private Rigidbody rigidbodyRef;

    // ============================================= Runtime =============================================
    private Vector3 recordedMoveDestination;
    private Vector3 recordedMoveStart;
    private Coroutine currentCameraAnimation;
    private Coroutine currentWallJumpCoroutine;

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
                cameraAnimator.transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    private void WallRun()
    {
        bool previousIsRunningLeft = isWallRunningLeft;
        bool previousIsRunningRight = isWallRunningRight;
        bool previousIsWallRunning = isWallRunning;
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
        }

        if (isWallRunningRight
            && !rightWallDetection.isObstructed
            || PlayerInputHandler.Instance.GetMoveInput().z <= 0f
            || rigidbodyRef.velocity.magnitude < 1f
        )
        {
            isWallRunningLeft = false;
            isWallRunningRight = false;
        }

        isWallRunning = isWallRunningLeft || isWallRunningRight;
        playerMoveControl.isWallRunning = isWallRunningLeft || isWallRunningRight;

        if (!previousIsRunningLeft && isWallRunningLeft)
        {
            SetCameraWallRun(true);
        }
        else if (!previousIsRunningRight && isWallRunningRight)
        {
            SetCameraWallRun(false);
        }
        else if (previousIsWallRunning && !isWallRunning)
        {
            ResetCamera();
        }

        if (isWallRunning)
        {
            rigidbodyRef.velocity = new Vector3(rigidbodyRef.velocity.x,
                upForce,
                rigidbodyRef.velocity.z);
            upForce -= wallRunUpForceChangeRate * Time.deltaTime;

            if (PlayerInputHandler.Instance.GetJumpKeyDown())
            {
                Vector3 horizonForce = Vector3.zero;
                if (isWallRunningLeft)
                {
                    horizonForce = Vector3.right * wallJumpHorizonForce *
                                   Vector3.Dot(transform.forward, Vector3.forward);
                }
                else if (isWallRunningRight)
                {
                    horizonForce = Vector3.left * wallJumpHorizonForce *
                                   Vector3.Dot(transform.forward, Vector3.forward);
                }

                rigidbodyRef.AddForce(transform.up * wallJumpUpForce, ForceMode.VelocityChange);

                if (currentWallJumpCoroutine != null)
                {
                    StopCoroutine(currentWallJumpCoroutine);
                }

                currentWallJumpCoroutine = StartCoroutine(WallJump(horizonForce));
            }

            if (playerMoveControl.IsGrounded)
            {
                isWallRunningLeft = false;
                isWallRunningRight = false;
                canWallRun = true;
            }
        }
    }

    private IEnumerator WallJump(Vector3 horizonForce)
    {
        float start = Time.time;
        float progress = (Time.time - start) / wallJumpMaxDuration;
        while (progress < 1f)
        {
            progress = (Time.time - start) / wallJumpMaxDuration;
            rigidbodyRef.AddForce(horizonForce * Time.deltaTime, ForceMode.VelocityChange);
            if (!leftWallDetection.isObstructed && !rightWallDetection.isObstructed)
            {
                isWallRunningLeft = false;
                isWallRunningRight = false;
                canWallRun = true;
            }

            yield return null;
        }
    }

    private void SetCameraWallRun(bool isLeft)
    {
        if (currentCameraAnimation != null)
        {
            StopCoroutine(currentCameraAnimation);
        }

        float sign = isLeft ? -1 : 1;
        currentCameraAnimation = StartCoroutine(
            MoveCameraToPosition(
                new Vector3(0, 0, 30f * sign)
            ));
    }

    private void ResetCamera()
    {
        if (currentCameraAnimation != null)
        {
            StopCoroutine(currentCameraAnimation);
        }

        currentCameraAnimation = StartCoroutine(
            MoveCameraToPosition(
                Vector3.zero
            ));
    }

    private IEnumerator MoveCameraToPosition(Vector3 dest)
    {
        float init = Time.time;
        float duration = cameraAnimationDuration;
        Vector3 start = cameraAnimator.transform.localEulerAngles;
        if (start.z > 180f)
        {
            start += Vector3.back * 360f;
        }

        float progress = 0;
        while (progress < 1f)
        {
            progress = (Time.time - init) / duration;
            cameraAnimator.transform.localEulerAngles = Vector3.Lerp(start, dest, progress);
            yield return null;
        }

        cameraAnimator.transform.localEulerAngles = dest;
    }
}