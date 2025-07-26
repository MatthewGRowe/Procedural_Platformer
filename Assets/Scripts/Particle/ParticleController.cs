using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem myParticles;
    bool played = false;

    //Turn on particle system when player approaches
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (played == false)
        {


            if (collision.tag == MyTags.PLAYER_TAG)
            {
                played = true;
                myParticles.Play();
                AudioManager.instance.Play(MyTags.SOUND_BONUS);
                
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            myParticles.Stop();
        }
    }

}
