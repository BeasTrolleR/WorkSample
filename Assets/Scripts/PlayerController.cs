using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{   
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
    
    //Public
    [HideInInspector] public Vector3 upAxis;

    //Component references
    private Rigidbody playerRigidbody;
    private InputManager inputManager;
    private CapsuleCollider playerCollider;

    //Movement
    private Quaternion playerRotation;
    private Vector3 playerVelocity;
    private Vector3 desiredVelocity;
    private Vector3 playerDisplacement;
    private Vector3 desiredPosition;
    private float maxSpeedChange;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Start()
    {
        
    }

    void FixedUpdate()
    {
        upAxis = -Physics.gravity.normalized;
            
        PlayerMove();
        
        //Jump Check
        if (inputManager.isJumping)
        {
            inputManager.isJumping = false;
            PlayerJump();
        }
    }

    private void Update()
    {
        
    }

    private void PlayerMove()
    {
        //Combining acceleration and velocity to acquire smoother movement
        desiredVelocity = new Vector3(inputManager.playerInput.x, 0f, inputManager.playerInput.z) * playerMoveSpeed;
        maxSpeedChange = playerAcceleration * Time.deltaTime;
        playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, desiredVelocity.z, maxSpeedChange);
        //Move player to a new position
        playerDisplacement = playerVelocity * Time.deltaTime;
        desiredPosition = transform.localPosition += playerDisplacement;

        //Check if player is in motion and rotate player accordingly
        if (playerVelocity != Vector3.zero)
        {
            //Store current player direction and rotate player accordingly, mathf.pow is used to avoid high numbers when tweaking
            playerRotation = Quaternion.LookRotation(playerVelocity, upAxis);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, Mathf.Pow(playerRotateSpeed,2)*Time.deltaTime);
        }
    }

    private void PlayerJump()
    {
        
    }
}
