using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximityNoise : MonoBehaviour
{
    //Makes a noise when player approaches

    public string soundToPlay;
    public float volume_0_to_1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
            AudioManager.instance.PlayAtSetVolume(soundToPlay, volume_0_to_1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
            AudioManager.instance.FadePlay(soundToPlay, 1);
    }

    public void StopNoise()
    {
        //If attached object is no longer active
        AudioManager.instance.StopPlay(soundToPlay);
    }

}
