using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{   
    //Character stats
    [Header("Character Settings")]
    [Range(1, 999)]
    [SerializeField] private int maxHealth = 10;
    [Range(1, 999)]
    [SerializeField] private int maxMana = 10;
    [Range (1, 99)]
    [SerializeField] private float playerMoveSpeed = 10f;
    [Range(1, 99)]
    [SerializeField] private float playerJumpHeight = 10f;

    //Components
    private Rigidbody playerRigidbody;
    private InputManager inputManager;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        inputManager = FindObjectOfType<InputManager>();
    }

    void FixedUpdate()
    {
        PlayerMove();
        PlayerJump();
    }

    private void Update()
    {

    }

    private void PlayerMove()
    {
        //Move
        playerRigidbody.MovePosition(transform.position + (inputManager.playerDirection * playerMoveSpeed * Time.deltaTime));
    }

    private void PlayerJump()
    {
        if (inputManager.isJumping)
        {
            inputManager.isJumping = false;
            playerRigidbody.velocity = Vector3.up.normalized * playerJumpHeight;
        }
    }
}
