using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //This is new and enables us to restart the level

public class BladeScript : MonoBehaviour
{
    public float rotationSpeed = 3f;  //How fast the blade rotates
    //Create two gameobjects and position them at the top point and the lowest point
    public Transform bottomPosition, topPosition;
    public float riseSpeed = 3f;  //How fast the blade rises or falls

    private Rigidbody2D myBody;  //This will enable us to move the blade up and down

    //Are we moving up or down
    private bool movingUp = false;  //Initially we will move down

    private void Start()
    {
        riseSpeed = Random.Range(1f, 8f);  //How fast the blade rises or falls
        myBody = GetComponent<Rigidbody2D>();  //Read the rigidBody component    
    }

    private void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, rotationSpeed));  //Move blade around


        //Deal with up down movement
        if (movingUp == true)  //If the blade is moving up
        {
            myBody.velocity = new Vector2(0, riseSpeed);

        }
        else //If blade is moving down
        {
            myBody.velocity = new Vector2(0, -riseSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)  //Called when we hit a collider
    {
        if (other.tag == MyTags.SAW_UP_TAG)   //Saw has gone up to the limit
        {
            movingUp = false;
        }
        else if (other.tag == MyTags.SAW_DOWN_TAG)    //Saw has gone down to the limit
        {
            movingUp = true;
        }
        else if (other.tag == MyTags.PLAYER_TAG && GameManager.instance.playerInvincible== false)   //Saw has hit the player
        {
            GameManager.instance.LoseALife(); //Restart lvl

        }
        else if (other.tag == MyTags.BULLET_TAG || (other.tag == MyTags.PLAYER_TAG && GameManager.instance.playerInvincible == true))
        {
            AudioManager.instance.StopPlay(MyTags.SOUND_BUZZ_SAW);
            AudioManager.instance.Play(MyTags.SOUND_ROCKSTRIKE, 1);
            Destroy(gameObject);
        }
    }
}
