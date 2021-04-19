using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [Range(0, 99)][Tooltip("How fast player rotates")]
    [SerializeField] private float playerRotSpeed = 10f;
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
    [Range(0, 99)][Tooltip("At what speed character snaps to ground")]
    [SerializeField] private float maxGroundSnapSpeed = 99f;
    [Range(0, 10)][Tooltip("Limiting how far to check for ground")]
    [SerializeField] private float maxGroundSnapDistance = 99f;
    
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
    private int lastGroundStep, lastJumpStep;
    private int jumpCount;
    private bool onGround => groundContactCount > 0;
    private float minGroundDotProduct;
    
    //Gravity
    private Vector3 upAxis;

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
        PlayerInput();
    }

    void FixedUpdate()
    {
        upAxis = -Physics.gravity.normalized;
        UpdateState();
        AdjustVelocity();
        GroundSnap();
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
        
        //Player Rotation
        Quaternion newRotation = quaternion.LookRotation(desiredVelocity, upAxis);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * playerRotSpeed);
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

    //Keep track of the jump phase and ground contact.
    private void UpdateState()
    {
        lastGroundStep += 1;
        lastJumpStep += 1;
        playerVelocity = playerRigidbody.velocity;
        
        if (onGround)
        {
            lastGroundStep = 0;
            jumpCount = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }
    }

    private void PlayerJump()
    {
        if (onGround || jumpCount < playerAirJumps || GroundSnap())
        {
            lastJumpStep = 0;
            jumpCount++;
            float jumpVelocity = Mathf.Sqrt(2f * Physics.gravity.magnitude * playerJumpHeight);
            float alignVelocity = Vector3.Dot(playerVelocity, contactNormal);
            
            //Limit upward velocity to avoid spamming jump and increase speed.
            //Also makes the jump more consistent and accurate in jump height even if u are already falling downwards.
            if (alignVelocity > 0f)
            {
                jumpVelocity = Mathf.Max(jumpVelocity - playerVelocity.y, 0f);
            }

            //Jump Player
            playerVelocity += contactNormal * jumpVelocity;
        }
    }

    //Keeps player snapped to ground
    private bool GroundSnap()
    {
        
        if (lastGroundStep > 1f || lastJumpStep <=2f)
        {
            return false;
        }
        
        //Abort snapping when current speed exceeds the max snap speed
        float speed = desiredVelocity.magnitude;
        if (speed > maxGroundSnapSpeed)
        {
            return false;
        }
        

        //Checking if there is any ground below to snap onto
        if (!Physics.Raycast(playerRigidbody.position, -upAxis, out RaycastHit hit))
        {
            return false;
        }
        
        //Using the surface normal vector to se if the surface hit count as ground
        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < minGroundDotProduct)
        {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        
        //Adjust velocity to align with ground
        float dot = Vector3.Dot(playerVelocity, hit.normal);

        if (dot > 0f)
        {
            playerVelocity = (playerVelocity - hit.normal * dot).normalized * speed;
        }

        return true;
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
            float UpDot = Vector3.Dot(upAxis, groundNormal);
            //Keep count of how many normals in contact
            if (UpDot >= minGroundDotProduct)
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
