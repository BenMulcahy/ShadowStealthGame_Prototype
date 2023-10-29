using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base controller for player inputs  built from brackeys tutorial as base: https://www.youtube.com/watch?v=_QajrabyTJc
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Ref to player character controller
    public CharacterController characterController;
    public Transform camContainer;
    Animator anim;

    //Key gained check
    public bool hasKey;

    //GroundCheck
    public Transform groundCheck;
    public float distToGround;
    public LayerMask WhatisGround;
    bool isGrounded;


    //Player Adjust Variables
    public float moveSpeed = 10f;
    public float crouchMoveSpeed = 5f;
    public float crouchHeight = 0.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float dashSlow = 0.5f;
    public float leanAngle = 20f;
    public float leanSpeed = 100f;

    float movementSpeed;

    [HideInInspector]
    public bool isCrouched;
    float currentLeanAngle;
    Vector3 velocity;

    //playerSkillsLock
    public bool canDash;
    public bool canLightSteal;
   

    private void Start()
    {
        Cursor.visible = false;
        movementSpeed = moveSpeed;
        anim = GetComponentInChildren<Animator>();
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distToGround, WhatisGround); //Checks in a sphere for anyobjects within the WhatIsGround layermask. distToGround is size of sphere

        //Reset velocity when grounded
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
        
        //Get X and Z inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //calculate move direction
        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * movementSpeed *Time.deltaTime);

        //Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        //jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            
        }

        //crouch
        if (Input.GetButtonDown("Crouch"))
        {
            camContainer.transform.localPosition = new Vector3(0, -0.644f, 0.125f);
            characterController.height = 1.65f;
            characterController.center = new Vector3(0, -0.22f, 0);
            movementSpeed = crouchMoveSpeed; //set movement speed to crouch movement speed
            isCrouched = true;
        }

        //Stand
        if (Input.GetButtonUp("Crouch"))
        {
            camContainer.transform.localPosition = new Vector3(0, -0.228f, 0.125f);
            characterController.height = 2;
            characterController.center = new Vector3(0, 0, 0);
            movementSpeed = moveSpeed; //set movement speed to standing move speed
            isCrouched = false;
        }
       

        //Dash
        if(Input.GetButton("Dash") && canDash == true && GetComponent<PlayerAbilities>().hasDashed == false) //prep dash if unlocked and not on cooldown
        {
            Time.timeScale = dashSlow;
            GetComponent<PlayerAbilities>().PrepDash();
        }

        if(Input.GetButtonUp("Dash") && canDash == true && GetComponent<PlayerAbilities>().hasDashed == false)//dash if unlocked and not on cooldown
        {
            Time.timeScale = 1f;
            StartCoroutine(GetComponent<PlayerAbilities>().Dash());
        }

        //Lean
        if (Input.GetKey(KeyCode.Q)) //lean left
        {
             currentLeanAngle = Mathf.MoveTowardsAngle(currentLeanAngle, leanAngle, leanSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E)) //lean right
        {
            currentLeanAngle = Mathf.MoveTowardsAngle(currentLeanAngle, -leanAngle, leanSpeed * Time.deltaTime);
        }
        else
        {
            currentLeanAngle = Mathf.MoveTowardsAngle(currentLeanAngle, 0f, leanSpeed * Time.deltaTime); //reset lean
        }
        camContainer.transform.localRotation = Quaternion.AngleAxis(currentLeanAngle, Vector3.forward); //move cam to lean angle

        //Attack
        if (Input.GetMouseButtonDown(0) && GetComponent<PlayerAbilities>().hasAttacked == false) //attack as long as attack is not on cooldown
        {
            anim.SetTrigger("Attack");
            GetComponent<PlayerAbilities>().Attack();
            
        }
        else anim.ResetTrigger("Attack");

        //StealthKill
        if (Input.GetKey(KeyCode.F) && GetComponent<PlayerAbilities>().hasStealthKilled == false) //stealth kill as long as steal kill is not on cooldown
        {
            GetComponent<PlayerAbilities>().StealthKillCheck();
        }
        if (!Input.GetKey(KeyCode.F)) GetComponent<PlayerAbilities>().StealthKillReset(); //reset stealth kill when releasing F

        //LightSteal
        if (Input.GetMouseButtonDown(1) && canLightSteal && GetComponent<PlayerAbilities>().hasStolenLight == false) //prep light steal if unlocked and not on cooldown
        {
            
            GetComponent<PlayerAbilities>().LightSteal();
        }

        if (Input.GetMouseButtonUp(1) && canLightSteal && GetComponent<PlayerAbilities>().hasStolenLight == false) //steal light if unlocked and not on cooldown
        {
            
            StartCoroutine(GetComponent<PlayerAbilities>().StolenLight());
        }
    }

}
