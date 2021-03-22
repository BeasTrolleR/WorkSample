using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{   
    //Editable character stats
    [Header("Character Settings")]
    [Range(1, 999)]
    [SerializeField] private int maxHealth = 10;
    [Range(1, 999)]
    [SerializeField] private int maxMana = 10;
    [Range (1, 99)]
    [SerializeField] private float playerMoveSpeed = 10f;
    [Range (1, 99)]
    [SerializeField] private float playerRotateSpeed = 10f;
    [Range(1, 99)]
    [SerializeField] private float playerJumpHeight = 10f;

    //Component references
    private Rigidbody playerRigidbody;
    private InputManager inputManager;
    private CapsuleCollider playerCollider;

    //Misc Variables
    private Quaternion playerRotation;
    private float distToGround;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Start()
    {
        //Using the player collider to determine distance to ground
        distToGround = playerCollider.bounds.extents.y;
    }

    void FixedUpdate()
    {
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
        IsPlayerGrounded();
    }

    private void PlayerMove()
    {
        //Move player according to input direction
        playerRigidbody.MovePosition(transform.position + (inputManager.playerDirection * (playerMoveSpeed * Time.deltaTime)));
        
        //Check if player is moving and rotate player accordingly
        if (inputManager.playerDirection != Vector3.zero)
        {
            //Store current player direction and rotate player accordingly, mathf.pow is used to avoid high numbers when tweaking
            playerRotation = Quaternion.LookRotation(inputManager.playerDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, Mathf.Pow(playerRotateSpeed,2)*Time.deltaTime);
        }
    }

    private void PlayerJump()
    {
        //Make the player jump using velocity
        if (IsPlayerGrounded())
        {
            playerRigidbody.velocity = Vector3.up.normalized * playerJumpHeight;
        }
    }

    private bool IsPlayerGrounded()
    {
        //Ground check, shooting a raycast a fixed distance towards the ground
        return Physics.Raycast(playerCollider.bounds.center, Vector3.down, distToGround + 0.1f);
    }
}
