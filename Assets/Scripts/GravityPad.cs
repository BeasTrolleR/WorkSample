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

        //Safety debugging in case dependency is broken.
        if (gManager == null)
        {
            Debug.LogError("There is no GravityManager in scene");
            EditorApplication.isPlaying = false;
        }
    }
    

    private void OnCollisionEnter(Collision other)
    {
        //Swap the physics gravity in GravityManager when player collide with pad.
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("player detected");
            cManager.camOffsetChange = !cManager.camOffsetChange;
            gManager.changeGravityDirection = !gManager.changeGravityDirection;
        }
    }
}
