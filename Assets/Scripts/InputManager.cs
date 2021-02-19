using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
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

    public Vector3 PlayerDirection
    {
        get
        { return playerDirection;}
        private set
        {playerDirection = value;}
    }
}
