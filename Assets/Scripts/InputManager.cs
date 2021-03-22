using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Toggle Gamepad/Keyboard")]
    [SerializeField] private bool useGamepad = false;

    //Editable Keyboard Input
    [Header("Keyboard Input")]
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    
    //Movement
    [HideInInspector] public Vector3 playerDirection;
    [HideInInspector] public bool isJumping, isNotJumping;
    
    //Misc Variables
    private Vector3 playerInput;

    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        //Directional Input (KEYBOARD)
        
        //Directional Input (GAMEPAD)
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerDirection = new Vector3(playerInput.x, 0f, playerInput.z).normalized;

        //Jump Input
        isJumping |= Input.GetKeyDown(jumpKey);

    }
}