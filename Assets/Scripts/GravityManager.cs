using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GravityManager : MonoBehaviour
{
    [Header("General Gravity Settings")] 
    [Range(0, 20)][Tooltip("How strong the gravity pulls")]
    [SerializeField] float gravityStrength = 9.81f;
    public bool changeGravityDirection = false;

    private void Start()
    {
    }

    private void Update()
    {
        GravitySwap();

    }

    private void GravitySwap()
    {
        if (changeGravityDirection == false)
        {
            Physics.gravity = new Vector3(0f, -gravityStrength, 0f);
        }
        else
        {
            Physics.gravity = new Vector3(0f, gravityStrength, 0f);
        }
    }
}
