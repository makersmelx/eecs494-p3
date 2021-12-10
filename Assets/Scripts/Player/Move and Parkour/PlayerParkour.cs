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
    public float wallJumpForwardForce = 100f;
    public float wallJumpUpForce = 10f;
    public float wallJumpMaxDuration = 1f;

    [Tooltip("How long it takes to rotate the camera")]
    public float cameraAnimationDuration = 0.25f;

    public float cameraRotationAngle = 15f;

    public bool isWallRunning;
    private bool isWallRunningLeft;
    private bool isWallRunningRight;
    private bool canWallRun = true;
    private float upForce;


    // ============================================= Parkour =============================================
    public bool isParkour;
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

    private Vector3 currentNormal = Vector3.zero;

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
        )
        {
            isWallRunningLeft = false;
            isWallRunningRight = false;
        }

        if (isWallRunningRight
            && !rightWallDetection.isObstructed
        )
        {
            isWallRunningLeft = false;
            isWallRunningRight = false;
        }

        isWallRunning = isWallRunningLeft || isWallRunningRight;


        // Detect that whether the player is wall running on current face of a wall. If the player reaches the end of a face, it will end wall running
        if (isWallRunning)
        {
            Vector3 normal = Vector3.zero;
            if (isWallRunningLeft)
            {
                normal = (transform.position - leftWallDetection.currentCollider.ClosestPoint(transform.position))
                    .normalized;
            }
            else if (isWallRunningRight)
            {
                normal = (transform.position - rightWallDetection.currentCollider.ClosestPoint(transform.position))
                    .normalized;
            }

            if (currentNormal.Equals(Vector3.zero))
            {
                currentNormal = normal;
            }
            else if ((currentNormal - normal).magnitude > 0.01f)
            {
                isWallRunning = false;
                isWallRunningLeft = false;
                isWallRunningRight = false;
            }
        }
        else
        {
            currentNormal = Vector3.zero;
        }

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
            Vector3 forwardNormal = Vector3.zero;

            if (isWallRunningLeft)
            {
                forwardNormal = Vector3.Cross(currentNormal, Vector3.up);
            }
            else if (isWallRunningRight)
            {
                forwardNormal = Vector3.Cross(Vector3.up, currentNormal);
            }

            float finalSpeed = playerMoveControl.CurrentMaxSpeed;
            Vector3 finalVelocity = forwardNormal * finalSpeed;
            Vector3 wallVelocity = Vector3.Dot(finalVelocity, currentNormal) * currentNormal;
            finalVelocity -= wallVelocity;
            rigidbodyRef.velocity = finalVelocity + upForce * Vector3.up;

            upForce -= wallRunUpForceChangeRate * Time.deltaTime;

            if (PlayerInputHandler.Instance.GetJumpKeyDown())
            {
                Vector3 forwardForce = forwardNormal * wallJumpForwardForce;
                Vector3 horizonForce = Vector3.zero;
                Vector3 jumpForce = transform.up * wallJumpUpForce;
                if (isWallRunningLeft)
                {
                    horizonForce = currentNormal * wallJumpHorizonForce;
                    print(horizonForce.normalized + "<<");
                }
                else if (isWallRunningRight)
                {
                    horizonForce = currentNormal * wallJumpHorizonForce;
                    print(horizonForce.normalized + ">>");
                }

                if (currentWallJumpCoroutine != null)
                {
                    StopCoroutine(currentWallJumpCoroutine);
                }

                currentWallJumpCoroutine = StartCoroutine(WallJump(forwardForce, horizonForce, jumpForce));
            }

            if (playerMoveControl.IsGrounded)
            {
                isWallRunningLeft = false;
                isWallRunningRight = false;
                canWallRun = true;
            }
        }
    }

    private IEnumerator WallJump(Vector3 forwardForce, Vector3 horizonForce, Vector3 jumpForce)
    {
        float start = Time.time;
        float progress = (Time.time - start) / wallJumpMaxDuration;

        PlayerAudio.Instance.PlayJumpSound();

        // rigidbodyRef.AddForce(forwardForce, ForceMode.VelocityChange);
        while (progress < 1f)
        {
            progress = (Time.time - start) / wallJumpMaxDuration;
            if (progress < 0.625f)
            {
                float coefficient = Mathf.Pow(0.625f - progress, 2);
                rigidbodyRef.AddForce(forwardForce * coefficient * Time.deltaTime, ForceMode.VelocityChange);
                rigidbodyRef.AddForce(horizonForce * coefficient * Time.deltaTime, ForceMode.VelocityChange);
                rigidbodyRef.AddForce(jumpForce * coefficient * Time.deltaTime, ForceMode.VelocityChange);
            }

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
            MoveCameraWithRotation(
                new Vector3(0, 0, cameraRotationAngle * sign)
            ));
    }

    private void ResetCamera()
    {
        if (currentCameraAnimation != null)
        {
            StopCoroutine(currentCameraAnimation);
        }

        Vector3 rotation = Vector3.zero - cameraAnimator.transform.localEulerAngles;
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
            dest += Vector3.forward * 360f;
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

    private IEnumerator MoveCameraWithRotation(Vector3 rotation)
    {
        float init = Time.time;
        float duration = cameraAnimationDuration;

        float progress = 0;
        while (progress < 1f)
        {
            progress = (Time.time - init) / duration;
            cameraAnimator.transform.Rotate(rotation / duration * Time.deltaTime);
            yield return null;
        }
    }
}