using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximityNoiseAndAttack : MonoBehaviour
{

    //Makes a noise when player approaches
    private Animator myAnim;  //Enable us to control animator
    public string soundToPlay;
    public GameObject appleBasket;
    private void Start()
    {
        //Register our animator
        myAnim = GetComponentInParent<Animator>();  
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        //Only works if boss is alive
        if (GetComponentInParent<MadDoctor>().iAmDead == false)
        {
            //Player has entered the arena
            if (collision.tag == MyTags.PLAYER_TAG && GameManager.instance.bossAttack == false)
            {
                
                AudioManager.instance.StopTheme();
                AudioManager.instance.Play(soundToPlay);
                myAnim.Play("attack");
                GameManager.instance.SetAmbiance();  //Darken the room
                GameManager.instance.bossAttack = true;  //Record so we can play appropriate sound effects from MadDoctorSquirter.cs
                appleBasket.SetActive(true); //Enable the basket of apples
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
           // AudioManager.instance.FadePlay(soundToPlay, 1);
            myAnim.Play("Idle");
        }
    }

    public void StopNoise()
    {
        //If attached object is no longer active
        AudioManager.instance.StopPlay(soundToPlay);
    }

}
