using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GravityManager : MonoBehaviour
{
    public bool gravityOn = false;
    private Vector3 gravityDirection;

    private void Start()
    {

    }

    private void Update()
    {
        GravitySwap();
    }

    private void GravitySwap()
    {
        gravityDirection = Physics.gravity;
        Debug.Log(gravityOn);
        if (gravityOn == false)
        {
            gravityDirection = new Vector3(0, -9.81f, 0);
        }
        else
        {
            gravityDirection = new Vector3(0, 9.81f, 0);
        }

    }
}
