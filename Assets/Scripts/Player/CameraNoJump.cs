using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraNoJump : MonoBehaviour
{
    //Used to follow a "cameraTarget" object so that camera
    //won't follow the player on jump
    float floorLevel;
    bool newJump = true;  //Only record the newjump once!
    public Transform groundCheck;

    private void Start()
    {
        //Make camera follow this (attached to player)
        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.m_Follow = this.transform;
    }

    private void Update()
    {
        if (GameManager.instance.onStairs == false) //Only use this script if we are NOT on the stairs otherwise it gets jerky
        {
            if (GameManager.instance.playerJumping && newJump)
            {
                newJump = false; //We have recorded the Y so no further records until we land
                floorLevel = transform.position.y;
            }
            if (GameManager.instance.playerJumping)
            {
                //Do not move this object on the Y, so camera remains on the floor!
                Vector3 myPosition = transform.position;
                myPosition.y = floorLevel; //Keep the camera level
                transform.position = myPosition;
            }
            else
            {//Make sure we follow the player
                transform.position = groundCheck.position;
            }
            if (GameManager.instance.playerJumping == false)
            {
                //Player has landed
                newJump = true;
            }
        }
    }
    private void LateUpdate()
    {
        
    }

}
