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
        MovePlayer(inputManager.PlayerDirection);
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        //Moves the player.
        playerRigidbody.MovePosition(transform.position + (moveDirection * playerMoveSpeed * Time.deltaTime));
    }
}
