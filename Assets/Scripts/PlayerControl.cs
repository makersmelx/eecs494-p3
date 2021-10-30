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

    public PlayerStates CurrentState;

    [Header("Physics")]
    public float MaxSpeed;
    public float BackwardsMovementSpeed;
    public float InAirControl;

    private float ActSpeed;

    public float Acceleration;
    public float Deccelleration;
    public float DirectionalControl;

    private float InAirTimer;
    private float GroundedTimer;
    private float AdjustmentAmt;

    [Header("Jumping")]
    public float JumpAmt;

    [Header("Turning")]
    public float TurnSpeed;
    public float TurnSpeedInAir;
    public float TurnSpeedOnWalls;

    public float LookUpSpeed;
    public Camera Head;

    private float YTurn;
    private float XTurn;

    public float MaxLookAngle;
    public float MinLookAngle;

    [Header("WallRuns")]
    public float WallRunTime = 1;
    private float ActWallTime = 0;
    public float WallRunUpwardsMovement = 4;
    public float WallRunSpeedAcceleration = 2f;

    [Header("Sliding")]
    public float PlayerCtrl;
    public float SlideSpeedLimit;
    public float SlideAmt;

    [Header("Ledge Grab")]
    public float PullUpTime = 0.5f;
    private Vector3 OriginPos;
    private Vector3 LedgePos;
    private float ActPullUpTime;

    [Header("Crouching")]
    private bool Crouch;
    private CapsuleCollider Cap;
    public float CrouchHeight;
    private float StandingHeight;
    public float CrouchSpeed;

    private PlayerCollision Coli;
    private Rigidbody Rb;


    void Start()
    {

        Coli = GetComponent<PlayerCollision>();
        Rb = GetComponent<Rigidbody>();
        Cap = GetComponent<CapsuleCollider>();
        StandingHeight = Cap.height;
        AdjustmentAmt = 1;
        Crouch = false;

    }


    void Update()
    {

        float XMOV = Input.GetAxis("Horizontal");
        float YMOV = Input.GetAxis("Vertical");


        //Debug.Log("XY INPUTS: " + XMOV.ToString() + "," + YMOV.ToString());

        if (CurrentState == PlayerStates.grounded)
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

                if (!Crouch)
                {
                    Debug.Log("Start crouch");
                    StartCrouching();
                }
            }
            else
            {
                bool check = Coli.CheckRoof(transform.up);

                if (!check && Crouch)
                {
                    Debug.Log("Stop crouch");
                    StopCrouching();
                }

            }

            // check on ground
            bool checkG = Coli.CheckFloors(-transform.up);
            if (!checkG)
            {
                InAir();
            }

        }

        else if (CurrentState == PlayerStates.inair)
        {

            // check ledge grab
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 Ledge = Coli.CheckLedges();
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

            bool checkG = Coli.CheckFloors(-transform.up);
            if (checkG && InAirTimer > 0.2f)
            {
                OnGround();
            }
        }

        else if (CurrentState == PlayerStates.onwall)
        {
            bool wall = CheckWall(XMOV, YMOV);

            if (!wall)
            {
                InAir();
                return;
            }

            bool onGround = Coli.CheckFloors(-transform.up);
            if (onGround)
            {
                OnGround();
            }
        }

        else if (CurrentState == PlayerStates.ledgegrab)
        {
            Rb.velocity = Vector3.zero;
        }
    }












    private void FixedUpdate()
    {
        float Del = Time.deltaTime;

        float XMOV = Input.GetAxis("Horizontal");
        float YMOV = Input.GetAxis("Vertical");

        float CamX = Input.GetAxis("Mouse X");
        float CamY = Input.GetAxis("Mouse Y");


        LookUpDown(CamY, Del);

        if (CurrentState == PlayerStates.grounded)
        {
            // increase timer
            if (GroundedTimer < 10)
                GroundedTimer += Del;

            // magnitude of inputs
            float inputMagnitude = new Vector2(XMOV, YMOV).normalized.magnitude;
            // get speed to apply to
            float targetSpeed = Mathf.Lerp(BackwardsMovementSpeed, MaxSpeed, YMOV);


            // check crouch
            if (Crouch)
            {
                targetSpeed = CrouchSpeed;
            }


            lerpSpeed(inputMagnitude, Del, targetSpeed);


            MovePlayer(XMOV, YMOV, Del, 1);
            TurnPlayer(CamX, Del, TurnSpeed);

            if (AdjustmentAmt < 1)
                AdjustmentAmt += Del * PlayerCtrl;
            else
                AdjustmentAmt = 1;


        }

        else if (CurrentState == PlayerStates.inair)
        {


            if (InAirTimer < 10)
                InAirTimer += Del;

            MovePlayer(XMOV, YMOV, Del, InAirControl);
            TurnPlayer(CamX, Del, TurnSpeedInAir);
        }

        else if (CurrentState == PlayerStates.onwall)
        {
            ActWallTime += Del;

            TurnPlayer(CamX, Del, TurnSpeedOnWalls);

            WallRunMovement(YMOV, Del);
        }

        else if (CurrentState == PlayerStates.ledgegrab)
        {
            ActPullUpTime += Del;
            float pullUpLerp = ActPullUpTime / PullUpTime;
            if (pullUpLerp < 0.5)
            {
                float lamt = pullUpLerp * 2;
                Vector3 LPos = new Vector3(OriginPos.x, LedgePos.y, OriginPos.z);
                transform.position = Vector3.Lerp(OriginPos, LPos, lamt);
            }
            else if (pullUpLerp <= 1)
            {
                if (OriginPos.y != LedgePos.y)
                {
                    OriginPos = new Vector3(OriginPos.x, LedgePos.y, OriginPos.z);
                }

                float lamt = (pullUpLerp - 0.5f) * 2f;
                transform.position = Vector3.Lerp(OriginPos, LedgePos, pullUpLerp);
            }
            else
            {
                OnGround();
            }
        }
    }













    void JumpUp()
    {
        Vector3 Vel = Rb.velocity;
        Vel.y = 0;

        Rb.velocity = Vel;

        Rb.AddForce(transform.up * JumpAmt, ForceMode.Impulse);

        InAir();
    }

    void lerpSpeed(float mag, float d, float speed)
    {
        float LaMT = speed * mag;
        float accel = Acceleration;
        if (mag == 0)
            accel = Deccelleration;

        ActSpeed = Mathf.Lerp(ActSpeed, LaMT, d * accel);
    }

    void MovePlayer(float hor, float ver, float d, float control)
    {
        Vector3 MovDir = transform.forward * ver + transform.right * hor;
        MovDir = MovDir.normalized;

        // if no input, stay original speed
        if (hor == 0 && ver == 0)
            MovDir = Rb.velocity.normalized;

        MovDir = MovDir * ActSpeed;
        MovDir.y = Rb.velocity.y;


        float acel = DirectionalControl * AdjustmentAmt * control;
        Vector3 LerpVel = Vector3.Lerp(Rb.velocity, MovDir, acel * d);
        Rb.velocity = LerpVel;
    }

    void TurnPlayer(float xamt, float d, float speed)
    {
        YTurn += xamt * d * speed;

        transform.rotation = Quaternion.Euler(0, YTurn, 0);
    }

    void LookUpDown(float YAmt, float d)
    {
        XTurn -= (YAmt * d) * LookUpSpeed;
        XTurn = Mathf.Clamp(XTurn, MinLookAngle, MaxLookAngle);

        Head.transform.localRotation = Quaternion.Euler(XTurn, 0, 0);
    }

    void InAir()
    {
        if (Crouch)
        {
            StopCrouching();
        }
        InAirTimer = 0;
        CurrentState = PlayerStates.inair;
    }

    void OnGround()
    {
        GroundedTimer = 0;
        ActWallTime = 0;
        CurrentState = PlayerStates.grounded;
    }

    void LedgeGrab(Vector3 LPos)
    {
        LedgePos = LPos;
        OriginPos = transform.position;
        ActPullUpTime = 0;
        CurrentState = PlayerStates.ledgegrab;
    }

    bool CheckWall(float XM, float YM)
    {
        if (XM == 0 && YM == 0)
            return false;

        if (ActWallTime > WallRunTime)
            return false;

        Vector3 WallDirection = transform.forward * YM + transform.right * XM;
        WallDirection = WallDirection.normalized;

        bool WallCol = Coli.CheckWalls(WallDirection);
        return WallCol;
    }

    void WallRun()
    {
        CurrentState = PlayerStates.onwall;
    }

    void WallRunMovement(float verticalMov, float D)
    {
        Vector3 MovDir = transform.up * verticalMov;
        MovDir = MovDir * WallRunUpwardsMovement;

        MovDir += transform.forward * ActSpeed;

        Vector3 lerpAmt = Vector3.Lerp(Rb.velocity, MovDir, WallRunSpeedAcceleration * D);
        Rb.velocity = lerpAmt;
    }

    void StartCrouching()
    {
        Crouch = true;
        Cap.height = CrouchHeight;

        if (ActSpeed > SlideSpeedLimit)
            SlideForward();
    }

    void StopCrouching()
    {
        Crouch = false;
        Cap.height = StandingHeight;
    }

    void SlideForward()
    {
        Debug.Log("In sliding");

        /*
        ActSpeed = SlideSpeedLimit;
        AdjustmentAmt = 0;
        */
        Vector3 Dir = Rb.velocity.normalized;

        Dir.y = 0;
        Rb.AddForce(Dir * SlideAmt, ForceMode.Impulse);

    }

}
