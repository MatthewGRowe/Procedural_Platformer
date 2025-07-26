using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GunPositioningScript : MonoBehaviour
{
    //Positions the gun and makes the particles sparkle when player approcahes
    public GameObject theParticles;  
    public GameObject theGun;
    Rigidbody2D myBody;
    public LayerMask groundLayer;
    bool played = false;

   

    private void Awake()
    {
        GetComponent<Rigidbody2D>().gravityScale = 8;
        transform.position = theGun.transform.position; //Keep objects together
        myBody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {


        //Coin  will start by falling, when it gets a distance above the floor it will stop falling
        if (Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer))
        {
            
            theGun.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            theGun.GetComponent<Rigidbody2D>().gravityScale = 1;
            myBody.bodyType = RigidbodyType2D.Static;
            myBody.gravityScale = 0;

            //Hide the gun
            Vector3 temp = theGun.transform.position;
            temp.z = -10;
            theGun.transform.position = temp;

            //Once landed move the particles
            theParticles.GetComponent<Transform>().position = transform.position;
        }
        else if (theGun.GetComponent<Rigidbody2D>().velocity.y == 0)  //We are stuck on something
        {
            
            theGun.GetComponent<Rigidbody2D>().velocity = new Vector2(theGun.GetComponent<Rigidbody2D>().velocity.x, 20);

        }

        if (GameManager.instance.playerHasDied)
        {
            RaceToStartPosition(); //Push coin quickly back to earth
        }


      
    }

    //Turn on particle system when player approaches
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (played == false)
        {


            if (collision.tag == MyTags.PLAYER_TAG)
            {

                //Unhide the gun
                Vector3 temp = theGun.transform.position;
                temp.z = 10;
                theGun.transform.position = temp;

                played = true;
                theParticles.GetComponent<ParticleSystem>().Play();
                AudioManager.instance.Play(MyTags.SOUND_BONUS);

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            theParticles.GetComponent<ParticleSystem>().Stop();
        }
    }

    private void RaceToStartPosition()
    {
        //Used after a player dies.
        theGun.GetComponent<Rigidbody2D>().gravityScale = 8;
     
    }
}
