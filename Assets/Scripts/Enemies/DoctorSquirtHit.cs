using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorSquirtHit : MonoBehaviour
{

    //Used to detect chemical spray hitting player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            
            AudioManager.instance.Play("Sizzle");
            GameManager.instance.DropHealth(0.1f);  //Damage to player
        }
    }
}
