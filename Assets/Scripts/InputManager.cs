using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Keybindings
    [SerializeField] private KeyCode jumpInput = KeyCode.Space;
    //Movement
    private Vector3 playerInput;
    private Vector3 playerDirection;

    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        //Directional Input
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerDirection = new Vector3(playerInput.x, 0f, playerInput.z).normalized;
    }

    public Vector3 MoveInput
    {
        get
        { return playerDirection;}
        private set
        {playerDirection = value;}
    }
    public KeyCode JumpInput
    {
        get
        { return jumpInput; }
        private set
        { jumpInput = value; }
    }

}