using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Editable Keyboard Input
    [Header("Player Keybindings")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    
    //Movement
    [HideInInspector] public bool isJumping, isNotJumping;
    
    //Misc Variables
    [HideInInspector]public Vector3 playerInput;

    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        //Directional Input, using ClampMagnitude instead of Normalize for diagonal movement input.
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

        //Jump Input check
        isJumping |= Input.GetKeyDown(jumpKey);
    }
}