using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Swaps the camera offset to Positive or Negative")]
    public bool camOffsetChange = false;
    private CinemachineTransposer offsetChange;
    
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
    
    private void CamOffset()
    {
        if (camOffsetChange == true)
        {
            offsetChange.m_FollowOffset = new Vector3(0f, -10f, -10f);;
        }
        else
        {
            offsetChange.m_FollowOffset = new Vector3(0f, 10f, -10f);
        }
    }
}
