using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GravityPad : MonoBehaviour
{
    private GravityManager gManager;
    private CameraManager cManager;

    private void Start()
    {
        gManager = FindObjectOfType<GravityManager>();
        cManager = FindObjectOfType<CameraManager>();
        
        //For easy debugging if the dependencies are lost.
        if (cManager == null)
            Debug.LogError("No CameraManager in scene");
        if (gManager == null)
            Debug.LogError("No GravityManager in scene");
    }
    

    private void OnCollisionEnter(Collision other)
    {
        //Swap the physics gravity in GravityManager when player collide with pad.
        if (other.gameObject.CompareTag("Player"))
        {
            cManager.camOffsetChange = !cManager.camOffsetChange;
            gManager.changeGravityDirection = !gManager.changeGravityDirection;
        }
    }
}
