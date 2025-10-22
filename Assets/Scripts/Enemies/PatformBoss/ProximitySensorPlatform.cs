using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorPlatform : MonoBehaviour
{
    //Attached to the Indian Platform Boss

    //Makes a noise when player approaches and enables the attack mode
    public bool canWeAttack = false;  //Picked up by the BigFork.cs script and enables the attack
    public string soundToPlay;
    public float volume_0_to_1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
            //Player has entered the arena
            if (collision.tag == MyTags.PLAYER_TAG)
            {

            //Play platform boss theme song
            AudioManager.instance.PlayAtSetVolume(soundToPlay, volume_0_to_1);
            canWeAttack = true;
               
            }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            //Fade out the music
            AudioManager.instance.FadePlay(soundToPlay, 1);
            canWeAttack = false;
            
        }
    }

    public void StopNoise()
    {
        //If attached object is no longer active
        AudioManager.instance.StopPlay(soundToPlay);
    }

}

