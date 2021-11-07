using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum PlayerStates
    {
        grounded,
        inair,
        onwall,
        ledgegrab,
    }

    public PlayerStates currentState;

    [Header("Physics")]
    public float maxSpeed;
    public float backwardsMovementSpeed;
    public float inAirControl;

    private float actualSpeed;

    public float acceleration;
    public float deceleration;
    public float directionalControl;

    private float inAirTimer;
    private float groundedTimer;
    private float adjustmentAmt;

    [Header("Jumping")]
    public float jumpAmt;

    [Header("Turning")]
    public float turnSpeed;
    public float turnSpeedInAir;
    public float turnSpeedOnWalls;

    public float lookUpSpeed;
    public Camera playerCamera;

    private float yTurn;
    private float xTurn;

    public float maxLookAngle;
    public float minLookAngle;

    [Header("WallRuns")]
    public float wallRunTime = 1;
    private float actWallTime = 0;
    public float wallRunUpwardsMovement = 4;
    public float wallRunSpeedacceleration = 2f;

    [Header("Sliding")]
    public float playerCtrl;
    public float slideSpeedLimit;
    public float slideAmt;

    [Header("Ledge Grab")]
    public float pullUpTime = 0.5f;
    private Vector3 originPos;
    private Vector3 ledgePos;
    private float actPullUpTime;

    [Header("Crouching")]
    private bool crouch;
    private CapsuleCollider cap;
    public float crouchHeight;
    private float standingHeight;
    public float crouchSpeed;

    private PlayerCollision playerCollision;
    private Rigidbody rb;


    void Start()
    {
        playerCollision = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody>();
        cap = GetComponent<CapsuleCollider>();
        standingHeight = cap.height;
        adjustmentAmt = 1;
        crouch = false;
    }


    void Update()
    {

        float XMOV = Input.GetAxis("Horizontal");
        float YMOV = Input.GetAxis("Vertical");


        //Debug.Log("XY INPUTS: " + XMOV.ToString() + "," + YMOV.ToString());

        if (currentState == PlayerStates.grounded)
        {

            // check for jumping
            if (Input.GetButtonDown("Jump"))
            {
                JumpUp();
            }

            // check for crouching
            if (Input.GetButton("Crouching"))

            {
                Debug.Log("Crouch input detected");

                if (!crouch)
                {
                    Debug.Log("Start crouch");
                    StartCrouching();
                }
            }
            else
            {
                bool check = playerCollision.CheckRoof(transform.up);

                if (!check && crouch)
                {
                    Debug.Log("Stop crouch");
                    StopCrouching();
                }

            }

            // check on ground
            bool checkG = playerCollision.CheckFloors(-transform.up);
            if (!checkG)
            {
                InAir();
            }

        }

        else if (currentState == PlayerStates.inair)
        {

            // check ledge grab
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 Ledge = playerCollision.CheckLedges();
                if (Ledge != Vector3.zero)
                {
                    LedgeGrab(Ledge);
                }
            }

            // check wall
            bool wall = CheckWall(XMOV, YMOV);

            if (wall)
            {
                WallRun();
                return;
            }

            bool checkG = playerCollision.CheckFloors(-transform.up);
            if (checkG && inAirTimer > 0.2f)
            {
                OnGround();
            }
        }

        else if (currentState == PlayerStates.onwall)
        {
            bool wall = CheckWall(XMOV, YMOV);

            if (!wall)
            {
                InAir();
                return;
            }

            bool onGround = playerCollision.CheckFloors(-transform.up);
            if (onGround)
            {
                OnGround();
            }
        }

        else if (currentState == PlayerStates.ledgegrab)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        float Del = Time.deltaTime;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float CamX = Input.GetAxis("Mouse X");
        float CamY = Input.GetAxis("Mouse Y");


        LookUpDown(CamY, Del);

        if (currentState == PlayerStates.grounded)
        {
            // increase timer
            if (groundedTimer < 10)
                groundedTimer += Del;

            // magnitude of inputs
            float inputMagnitude = new Vector2(horizontalInput, verticalInput).normalized.magnitude;
            // get speed to apply to
            float targetSpeed = Mathf.Lerp(backwardsMovementSpeed, maxSpeed, verticalInput);
            // check crouch
            if (crouch)
            {
                targetSpeed = crouchSpeed;
            }


            lerpSpeed(inputMagnitude, Del, targetSpeed);


            MovePlayer(horizontalInput, verticalInput, Del, 1);
            TurnPlayer(CamX, Del, turnSpeed);

            if (adjustmentAmt < 1)
                adjustmentAmt += Del * playerCtrl;
            else
                adjustmentAmt = 1;


        }

        else if (currentState == PlayerStates.inair)
        {


            if (inAirTimer < 10)
                inAirTimer += Del;

            MovePlayer(horizontalInput, verticalInput, Del, inAirControl);
            TurnPlayer(CamX, Del, turnSpeedInAir);
        }

        else if (currentState == PlayerStates.onwall)
        {
            actWallTime += Del;

            TurnPlayer(CamX, Del, turnSpeedOnWalls);

            WallRunMovement(verticalInput, Del);
        }

        else if (currentState == PlayerStates.ledgegrab)
        {
            actPullUpTime += Del;
            float pullUpLerp = actPullUpTime / pullUpTime;
            if (pullUpLerp < 0.5)
            {
                float lamt = pullUpLerp * 2;
                Vector3 LPos = new Vector3(originPos.x, ledgePos.y, originPos.z);
                transform.position = Vector3.Lerp(originPos, LPos, lamt);
            }
            else if (pullUpLerp <= 1)
            {
                if (originPos.y != ledgePos.y)
                {
                    originPos = new Vector3(originPos.x, ledgePos.y, originPos.z);
                }

                float lamt = (pullUpLerp - 0.5f) * 2f;
                transform.position = Vector3.Lerp(originPos, ledgePos, pullUpLerp);
            }
            else
            {
                OnGround();
            }
        }
    }

    void JumpUp()
    {
        Vector3 Vel = rb.velocity;
        Vel.y = 0;

        rb.velocity = Vel;

        rb.AddForce(transform.up * jumpAmt, ForceMode.Impulse);

        InAir();
    }

    void lerpSpeed(float mag, float d, float speed)
    {
        float LaMT = speed * mag;
        float accel = acceleration;
        if (mag == 0)
            accel = deceleration;
        actualSpeed = Mathf.Lerp(actualSpeed, LaMT, d * accel);
    }

    void MovePlayer(float hor, float ver, float d, float control)
    {
        Vector3 MovDir = transform.forward * ver + transform.right * hor;
        MovDir = MovDir.normalized;

        // if no input, stay original speed
        if (hor == 0 && ver == 0)
            MovDir = rb.velocity.normalized;

        MovDir = MovDir * actualSpeed;
        MovDir.y = rb.velocity.y;


        float acel = directionalControl * adjustmentAmt * control;
        Vector3 LerpVel = Vector3.Lerp(rb.velocity, MovDir, acel * d);
        rb.velocity = LerpVel;
    }

    void TurnPlayer(float xamt, float d, float speed)
    {
        yTurn += xamt * d * speed;

        transform.rotation = Quaternion.Euler(0, yTurn, 0);
    }

    void LookUpDown(float YAmt, float d)
    {
        xTurn -= (YAmt * d) * lookUpSpeed;
        xTurn = Mathf.Clamp(xTurn, minLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xTurn, 0, 0);
    }

    void InAir()
    {
        if (crouch)
        {
            StopCrouching();
        }
        inAirTimer = 0;
        currentState = PlayerStates.inair;
    }

    void OnGround()
    {
        groundedTimer = 0;
        actWallTime = 0;
        currentState = PlayerStates.grounded;
    }

    void LedgeGrab(Vector3 LPos)
    {
        ledgePos = LPos;
        originPos = transform.position;
        actPullUpTime = 0;
        currentState = PlayerStates.ledgegrab;
    }

    bool CheckWall(float XM, float YM)
    {
        if (XM == 0 && YM == 0)
            return false;

        if (actWallTime > wallRunTime)
            return false;

        Vector3 WallDirection = transform.forward * YM + transform.right * XM;
        WallDirection = WallDirection.normalized;

        bool WallCol = playerCollision.CheckWalls(WallDirection);
        return WallCol;
    }

    void WallRun()
    {
        currentState = PlayerStates.onwall;
    }

    void WallRunMovement(float verticalMov, float D)
    {
        Vector3 MovDir = transform.up * verticalMov;
        MovDir = MovDir * wallRunUpwardsMovement;

        MovDir += transform.forward * actualSpeed;

        Vector3 lerpAmt = Vector3.Lerp(rb.velocity, MovDir, wallRunSpeedacceleration * D);
        rb.velocity = lerpAmt;
    }

    void StartCrouching()
    {
        crouch = true;
        cap.height = crouchHeight;

        if (actualSpeed > slideSpeedLimit)
            SlideForward();
    }

    void StopCrouching()
    {
        crouch = false;
        cap.height = standingHeight;
    }

    void SlideForward()
    {
        Debug.Log("In sliding");

        /*
        actualSpeed = SlideSpeedLimit;
        adjustmentAmt = 0;
        */
        Vector3 Dir = rb.velocity.normalized;

        Dir.y = 0;
        rb.AddForce(Dir * slideAmt, ForceMode.Impulse);

    }

}
