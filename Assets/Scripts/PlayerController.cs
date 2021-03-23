using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    
    //Editable keybindings
    [Header("Player Keybindings")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    //Editable character stats
    [Header("Character Movement Settings")] 
    [Range(0, 99)][Tooltip("How fast player moves in a direction")]
    [SerializeField] private float playerMoveSpeed = 10f;
    [Range(0, 99)][Tooltip("How fast player accelerate when moving.")]
    [SerializeField] private float playerAcceleration = 10f;
    [Range(0, 5)][Tooltip("Number of Air Jumps allowed.")]
    [SerializeField] private int playerAirJumps = 0;
    [Range(1, 99)][Tooltip("Height in meters.")]
    [SerializeField] private float playerJumpHeight = 10f;
    [Range(0, 99)][Tooltip("How fast player accelerate while in air.")]
    [SerializeField] private float playerAirAcceleration = 1f;
    
    [Header("Ground Settings")] 
    [Range(0, 90)][Tooltip("Max angle on what is considered ground")]
    [SerializeField] private float maxGroundAngle = 25f;
    
    //Player Input
    private Vector3 playerInput;
    private bool isJumping;
    
    //Component references
    private Rigidbody playerRigidbody;

    //Movement
    private Vector3 playerVelocity;
    private Vector3 playerDisplacement;
    private Vector3 desiredVelocity;
    private Vector3 contactNormal;
    private float maxSpeedChange;

    //Misc
    private int groundContactCount;
    private int jumpCount;
    private bool onGround => groundContactCount > 0;
    private float minGroundDotProduct;

    #endregion

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    
    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        OnValidate();
    }
    
    void Update()
    {
        Debug.Log(groundContactCount);
        PlayerInput();
    }

    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        playerRigidbody.velocity = playerVelocity;
        ClearState();

    }

    private void PlayerInput()
    {
        //Directional Input, using ClampMagnitude instead of Normalize for diagonal movement input.
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

        //Jump Input check
        isJumping |= Input.GetKeyDown(jumpKey);
        
        //Using player input for desired velocity
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.z) * playerMoveSpeed;
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        
        float currentX = Vector3.Dot(playerVelocity, xAxis);
        float currentZ = Vector3.Dot(playerVelocity, zAxis);
        
        //Adjust velocity & use different acceleration settings bases on if grounded or not
        var acceleration = onGround ? playerAcceleration : playerAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        
        playerVelocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        //Jump Check
        if (isJumping)
        {
            isJumping = false;
            PlayerJump();
        }
        
    }

    //Used to reset normal and ground contact count
    private void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    //Keep track of the jump phase
    private void UpdateState()
    {
        playerVelocity = playerRigidbody.velocity;
        
        if (onGround)
        {
            jumpCount = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    private void PlayerJump()
    {
        if (onGround || jumpCount < playerAirJumps)
        {
            jumpCount++;
            float jumpVelocity = Mathf.Sqrt(-2f * Physics.gravity.y * playerJumpHeight);
            float alignVelocity = Vector3.Dot(playerVelocity, contactNormal);
            
            //Limit upward velocity to avoid spamming jump and increase speed.
            //Also makes the jump more consistent and accurate in jump height even if u are already falling downwards.
            if (alignVelocity > 0f)
            {
                jumpVelocity = Mathf.Max(jumpVelocity - playerVelocity.y, 0f);
            }

            //Jump Player
            playerVelocity += contactNormal * jumpVelocity;
            //(playerVelocity.y += jumpVelocity;
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    //Ground check based on normal direction of collided object.
    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 groundNormal = collision.GetContact(i).normal;
            
            //Keep count of how many normals in contact
            if (groundNormal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += groundNormal;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }
}
