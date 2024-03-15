using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //########## GLOBAL VARIABLES ##########
    [SerializeField] private float maxSpeed;
    [SerializeField] private float stamina;

    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform followTargetPos;

    bool isGrounded;
    bool hasControl = true;
    public bool InAction { get; private set; }
    public bool IsOnLedge { get; set; }

    public bool IsHanging { get; set; }
    public LedgeData LedgeData { get; set; }

    public bool IsAiming { get; set; }

    [HideInInspector] public bool isStanded;
    [HideInInspector] public bool isCrouched;
    [HideInInspector] public bool isProned;

    float horizontalInput;
    float verticalInput;

    Vector3 direction;
    Vector3 MoveDirection;
    Vector3 MoveDirLedge;
    public float speed;
    public float fallSpeed;
    public float fallTime = 0;
    float inputMagnitude;
    float targetAngle;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    float angle;
    Vector3 velocity;



    //########## COMPONENT REFERENCES ##########
    [SerializeField] CapsuleCollider headCollider;
    private CharacterController pController;
    [HideInInspector] public Animator pAnimation;
    public new Transform camera;
    EnvironmentScanner environmentScanner;

    private void Start()
    {
        pController = GetComponent<CharacterController>();
        pAnimation = GetComponent<Animator>();
        environmentScanner = GetComponent<EnvironmentScanner>();

    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");   
        verticalInput = Input.GetAxis("Vertical");

        if (!hasControl)
            return;

        if (IsHanging) return;


        GroundCheck();
        if (!isGrounded)
        {
            fallTime += Time.deltaTime;

            if (fallTime > 0.25) 
            {
                pAnimation.SetFloat("FallDelay",1.0f);
            }
        }
        else 
        {
            fallTime = 0;
            pAnimation.SetFloat("FallDelay", 0f);
        }

        AnimationStateControl();

    }

    //########## ANIMATION STATES ##########
    private void Stand()
    {
        isStanded = true;
        isCrouched = false;
        isProned = false;

        pController.center = new Vector3(0,1,0);
        pController.height = 1.8f;
        pController.radius = 0.3f;
        pAnimation.SetBool("isStanded", true);
        if (Input.GetKey(KeyCode.Joystick1Button11) && stamina > 0)
        {
            if (maxSpeed < 3.5)
                maxSpeed += 0.1f;
            stamina -= 0.2f;

        }
        else
        {
            maxSpeed = 2f;
            if (stamina < 100)
                stamina += 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            pAnimation.SetBool("isCrouched", true);
            pAnimation.SetBool("isStanded", false);
            pAnimation.SetBool("isProned", false);
        }

        Move();
    }

    private void Crouch()
    {

        isStanded = false;
        isCrouched = true;
        isProned = false;

        maxSpeed = 1.5f;
        pController.center = new Vector3(0f, 0.6f, 0.3f);
        pController.height = 1f;
        pController.radius = 0.3f;

        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
            pAnimation.SetBool("isStanded", false);
            pAnimation.SetBool("isCrouched", false);
            pAnimation.SetBool("isProned", true);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            pAnimation.SetBool("isCrouched", false);
            pAnimation.SetBool("isStanded", true);
            pAnimation.SetBool("isProned", false);
        }

        Move();
    }

    private void Prone()
    {
        isStanded = false;
        isCrouched = false;
        isProned = true;

        pAnimation.SetBool("isProned", true);
        maxSpeed = 0.8f;
        pController.center = new Vector3(0f, 0.2f, 0.3f);
        pController.height = 0.4f;
        pController.radius = 0.2f;

        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
            pAnimation.SetBool("isStanded", false);
            pAnimation.SetBool("isProned", false);
            pAnimation.SetBool("isCrouched", true);
        }


        Move();
    }

    //########## ANIMATION CONTROL ##########
    private void AnimationStateControl()
    {
        //TASK: if not grounded then play fall animation
        pAnimation.SetBool("isGrounded", isGrounded);

        if (!pAnimation.GetBool("isCrouched") && !pAnimation.GetBool("isProned"))
        {
            pAnimation.SetBool("isStanded", true);
            pAnimation.SetBool("isCrouched", false);
            pAnimation.SetBool("isProned", false);
            Stand();

        }
        else if (!pAnimation.GetBool("isProned") && !pAnimation.GetBool("isStanded"))
        {
            pAnimation.SetBool("isStanded", false);
            pAnimation.SetBool("isCrouched", true);
            pAnimation.SetBool("isProned", false);
            Crouch();
        }
        else if (!pAnimation.GetBool("isCrouched") && !pAnimation.GetBool("isStanded"))
        {
            pAnimation.SetBool("isStanded", false);
            pAnimation.SetBool("isCrouched", false);
            pAnimation.SetBool("isProned", true);
            Prone();
        }

    }

    //########## INPUT MOVEMENT ##########
    private void Move()
    {
        if (!IsAiming) 
        {
            direction = new Vector3(horizontalInput, 0, verticalInput);
            inputMagnitude = Mathf.Clamp01(direction.magnitude) * maxSpeed;
            speed = inputMagnitude * maxSpeed;
            pAnimation.SetFloat("input magnitude", inputMagnitude, 0.05f, Time.deltaTime);
            if (isGrounded && direction.magnitude >= 0.1f)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                MoveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                MoveDirLedge = MoveDirection;

                pController.Move(velocity * Time.deltaTime);
            }

            if (!isGrounded)
            {
                fallSpeed += Physics.gravity.y * Time.deltaTime;
                pController.Move(new Vector3(velocity.x, fallSpeed, velocity.z) * Time.deltaTime);
                velocity = transform.forward * speed / 2; //test
            }
            else
            {
                fallSpeed = -0.1f;
                velocity = MoveDirection.normalized * speed; //test
                IsOnLedge = environmentScanner.LedgeCheck(MoveDirection, out LedgeData ledgeData);

                if (IsOnLedge)
                {
                    LedgeData = ledgeData;
                    LedgeMovement();
                }
            }

        }
        //aiming movement here if else or just else

       

    }

    void GroundCheck() 
    {
        isGrounded =  Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);

    }

    void LedgeMovement() 
    {
        
        float signedAngle =  Vector3.SignedAngle(LedgeData.surfaceHit.normal, MoveDirection, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(MoveDirection, transform.forward) >= 80) 
        {
            velocity = Vector3.zero;
            return;

        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            MoveDirLedge = Vector3.zero;
        }
        else if (angle < 90) 
        {
            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);
            velocity = velocity.magnitude * dir;
            MoveDirLedge = dir;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null, 
        Quaternion targetRotation = new Quaternion(), bool rotate = false, float postDelay = 0f, 
        bool mirror = false)
    {
        InAction = true;
       

        pAnimation.SetBool("mirrorAction", mirror);
        pAnimation.CrossFadeInFixedTime(animName, 0.2f);
        yield return null;

        var AnimState = pAnimation.GetNextAnimatorStateInfo(0);
        if (!AnimState.IsName(animName))
            Debug.LogError("The action animation is wrong!");

        float rotateStartTime = (matchParams != null) ? matchParams.startTime : 0;

        float timer = 0f;
        while (timer <= AnimState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / AnimState.length;

            //rotate the player towards the obstacle
            if (rotate && timer > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angle * Time.deltaTime);

            //TargetMatching
            if (matchParams != null)
                MatchTarget(matchParams);

            if (pAnimation.IsInTransition(0) && timer > 0.5f)
                break;


            yield return null;
        }

        yield return new WaitForSeconds(postDelay);

       
        InAction = false;

    }

    void MatchTarget(MatchTargetParams mp)
    {
        if (pAnimation.isMatchingTarget || pAnimation.IsInTransition(0)) return;

        pAnimation.MatchTarget(mp.pos, transform.rotation, mp.bodyPart,
            new MatchTargetWeightMask(mp.posWeight, 0), mp.startTime, mp.targetTime);
    }

    public void SetControl(bool hasControl) 
    {
        this.hasControl = hasControl;
        pController.enabled = hasControl;

        if (!hasControl) 
        {
            pAnimation.SetFloat("input magnitude", 0);
        }

    }

    public void EnableCharacterController(bool enabled) 
    {
        pController.enabled = enabled;
    }

    public void ResetTargetRotation() 
    {
        //delete this function
    }

    public bool HasControl 
    {
        get => hasControl;
        set => hasControl = value;
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }


    public float Angle => transform.eulerAngles.y;

}


public class MatchTargetParams 
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}

//########## ISSUES ##########
//stamina only recovers during StandState
//only one button should switch the states
// SORTED player should know when is grounded or not
// SORTED run animation (not sprint) should be played faster at his max magnitude/speed
// SORTED walk animation issue = apparently fixed if removed .normalized from direction (line 143) from Move()
// SORTED tweak camera settings/orbits to improve gameplay
//comment more this code cause it's getting messy
