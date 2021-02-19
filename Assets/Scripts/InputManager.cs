using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Input
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    //Movement
    private Vector3 playerInput;
    [HideInInspector] public Vector3 playerDirection;
    [HideInInspector] public bool isJumping;

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

        //Jump
        isJumping |= Input.GetKeyDown(jumpKey);
    }
}