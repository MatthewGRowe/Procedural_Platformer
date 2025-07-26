using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public bool stairStart;  //Set by Object Placer script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            //If player is in the zone
            if (stairStart)
            {
                GameManager.instance.onStairs = true; //This value will turn off the CameraNoJump script
            }
            else
            {
                GameManager.instance.onStairs = false; //This value will turn on the CameraNoJumpScript
            }
        }
    }
}
