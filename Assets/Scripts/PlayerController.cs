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
    [Header("Character Settings")] 
    [Range(1, 99)]
    [SerializeField] private float playerMoveSpeed = 10f;
    [Range(1, 99)]
    [SerializeField] private float playerAcceleration = 10f;
    [Range (1, 99)]
    [SerializeField] private float playerRotateSpeed = 10f;
    [Range(1, 99)]
    [SerializeField] private float playerJumpHeight = 10f;
    
    //Component references
    private Rigidbody playerRigidbody;
    private InputManager inputManager;
    private CapsuleCollider playerCollider;

    //Movement
    private Quaternion playerRotation;
    private Vector3 playerVelocity;
    private Vector3 playerDisplacement;
    private Vector3 desiredVelocity;
    //private Vector3 desiredPosition;
    private float maxSpeedChange;
    #endregion

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        inputManager = FindObjectOfType<InputManager>();
    }
    
    void Update()
    {
        //Using player input for desired velocity
        desiredVelocity = new Vector3(inputManager.playerInput.x, 0f, inputManager.playerInput.z) * playerMoveSpeed;
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
    
    private void PlayerMove()
    {
        //Adjust physics/velocity
        playerVelocity = playerRigidbody.velocity;
        maxSpeedChange = playerAcceleration * Time.deltaTime;
        playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, desiredVelocity.z, maxSpeedChange);
        
        //Jump Check
        if (inputManager.isJumping)
        {
            inputManager.isJumping = false;
            PlayerJump();
        }
        
        //Override Rigidbody and apply adjusted velocity
        playerRigidbody.velocity = playerVelocity;

        //Rotate if player is moving
        if (playerVelocity.x != 0f || playerVelocity.z != 0f)
        {
            playerRotation = Quaternion.LookRotation(new Vector3(playerVelocity.x, 0f, playerVelocity.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, Mathf.Pow(playerRotateSpeed,2)*Time.deltaTime);
        }
    }

    private void PlayerJump()
    {
        playerVelocity.y += playerJumpHeight;
    }
}
