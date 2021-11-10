using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkour : MonoBehaviour
{
    // ============================================= Animator =============================================
    public Animator cameraAnimator;

    // ============================================= Drag and Friction =============================================
    public float dragOnGround = 5f;
    public float dragInAir = 0.2f;
    
    
    // Detection Reference
    public ObstructionDetection climbObjectDetection; //checks for climb object
    public ObstructionDetection climbObstructionDetection; //checks if theres somthing in front of the object e.g walls that will not allow the player to climb
    
    // ============================================= Parkour =============================================
    private bool isParkour;
    private float parkourProgress;
    private float currentParkourMoveTime;
    
    // ============================================= Vault =============================================
    
    // ============================================= Climb =============================================
    private bool canClimb;
    public float climbTime; //how long the vault takes
    public Transform climbEndPoint;
    // ============================================= Reference =============================================
    private PlayerMoveControl playerMoveControl;
    private Rigidbody rigidbodyRef;

    // ============================================= Runtime =============================================
    private Vector3 recordedMoveDestination; 
    private Vector3 recordedMoveStart;
    private Quaternion initialRotation;

    private void Start()
    {
        playerMoveControl = GetComponent<PlayerMoveControl>();
        rigidbodyRef = GetComponent<Rigidbody>();
        initialRotation = playerMoveControl.playerCamera.transform.rotation;
    }

    private void Update()
    {
        SetDrag();
        Climb();
        
       PerFormParkour();
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
            && !climbObstructionDetection.isObstructed)
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
            cameraAnimator.CrossFade("Climb",0.1f);
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
                playerMoveControl.playerCamera.transform.rotation = initialRotation;
            }
        }
    }
}
