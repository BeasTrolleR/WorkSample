using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Swaps the camera offset to Positive or Negative")]
    public bool camOffsetChange = false;
    
    private CinemachineTransposer offsetChange;
    
    private float offsetValue = 10f;
    [Range(0f, 1f)]
    private float offsetLerpAmount = 1f;

    // Start is called before the first frame update
    void Start()
    {
        offsetChange = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        CamOffset();
    }
    
    //Interpolates the cam offset vector to give a smooth cam transition when changing gravity.
    private void CamOffset()
    {
        Vector3 posOffset = new Vector3(0f, offsetValue, -10f);
        Vector3 negOffset = new Vector3(0f, -offsetValue, -10f);

        if (camOffsetChange == true)
        {
            if (offsetLerpAmount <= 0f)
            {
                offsetLerpAmount = 0f;
            }
            else if (offsetLerpAmount <= 1f)
            {
                offsetLerpAmount -= Time.deltaTime;
            }
        }
        else
        {
            if (offsetLerpAmount >= 1f)
            {
                offsetLerpAmount = 1f;
            }
            else if (offsetLerpAmount >= 0f)
            {
                offsetLerpAmount += Time.deltaTime;
            }
        }
        
        offsetChange.m_FollowOffset = Vector3.Lerp(negOffset, posOffset, offsetLerpAmount);
    }
}
