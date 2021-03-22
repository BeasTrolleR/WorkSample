using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [HideInInspector] public Vector3 upAxis;
    
    // Start is called before the first frame update
    void Start()
    {
        upAxis = -Physics.gravity.normalized;

    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
