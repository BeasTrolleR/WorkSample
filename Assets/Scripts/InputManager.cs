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
        //Directional Input
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerDirection = new Vector3(playerInput.x, 0f, playerInput.z);

        //Jump Input check
        isJumping |= Input.GetKeyDown(jumpKey);
    }
}