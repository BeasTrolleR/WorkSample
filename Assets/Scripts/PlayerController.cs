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
    //Editable character stats
    [Header("Character Movement Settings")] 
    [Range(0, 99)][Tooltip("How fast player moves in a direction")]
    [SerializeField] private float playerMoveSpeed = 10f;
    [Range(0, 99)][Tooltip("How fast player accelerate when moving.")]
    [SerializeField] private float playerAcceleration = 10f;
    [Range (0, 99)][Tooltip("How fast player rotates towards movement direction.")]
    [SerializeField] private float playerRotateSpeed = 10f;
    [Range(0, 5)][Tooltip("Number of Air Jumps allowed.")]
    [SerializeField] private int playerAirJumps = 0;
    [Range(1, 99)][Tooltip("Height in meters.")]
    [SerializeField] private float playerJumpHeight = 10f;
    [Range(0, 99)][Tooltip("How fast player accelerate while in air.")]
    [SerializeField] private float playerAirAcceleration = 1f;
    
    [Header("Ground Settings")] 
    [Range(0, 90)][Tooltip("Max angle on what is considered ground")]
    [SerializeField] private float maxGroundAngle = 25f;
    
    //Component references
    private Rigidbody playerRigidbody;
    private InputManager inputManager;
    //private CapsuleCollider playerCollider;

    //Movement
    private Quaternion playerRotation;
    private Vector3 playerVelocity;
    private Vector3 playerDisplacement;
    private Vector3 desiredVelocity;
    private float maxSpeedChange;
    
    //Misc
    private int jumpCount;
    private bool onGround;
    private float minGroundDotProduct;
    
    #endregion

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    
    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        inputManager = FindObjectOfType<InputManager>();
        OnValidate();
        //playerCollider = GetComponent<CapsuleCollider>();
    }
    
    void Update()
    {
        //Using player input for desired velocity
        desiredVelocity = new Vector3(inputManager.playerInput.x, 0f, inputManager.playerInput.z) * playerMoveSpeed;
    }

    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        PlayerMove();
    }

    private void AdjustVelocity()
    {
        //Adjust velocity & use different acceleration settings bases on if grounded or not
        var acceleration = onGround ? playerAcceleration : playerAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, desiredVelocity.z, maxSpeedChange);
        
        //Jump Check
        if (inputManager.isJumping)
        {
            inputManager.isJumping = false;
            PlayerJump();
        }
        
        onGround = false;
    }

    private void UpdateState()
    {
        playerVelocity = playerRigidbody.velocity;
        
        if (onGround)
        {
            jumpCount = 0;
        }
    }

    private void PlayerMove()
    {
        //Rotate if player is moving
        if (playerVelocity.x != 0f || playerVelocity.z != 0f)
        {
            playerRotation = Quaternion.LookRotation(new Vector3(playerVelocity.x, 0f, playerVelocity.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, Mathf.Pow(playerRotateSpeed,2)*Time.deltaTime);
        }
        
        //Move Player
        playerRigidbody.velocity = playerVelocity;
    }

    private void PlayerJump()
    {
        if (onGround || jumpCount < playerAirJumps)
        {
            jumpCount++;
            //Want jump height do be consistent and unaffected by downwards forces, so resetting velocity.Y before jumping
            playerVelocity.y = 0f;
            //This allows me to get a more accurate jump height. For example, a playerJumpHeight of 1 is very close to 1 meter in game.
            playerVelocity.y += Mathf.Sqrt(-2 * Physics.gravity.y * playerJumpHeight);   
        }
    }

    //Ground check based on normal direction of collided object.
    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 contactNormal = collision.GetContact(i).normal;
            onGround |= contactNormal.y >= minGroundDotProduct;
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
