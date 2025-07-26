using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClubScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D target)
    {
        if (gameObject.tag != MyTags.BULLET_TAG)
        {
            return; //We are not attacking so ignore the hit
        }
        if (target.gameObject.tag == MyTags.ENEMY_TAG || target.gameObject.tag == MyTags.PUPPY_TAG)
        {
            if (target.gameObject.tag == MyTags.PUPPY_TAG)
            {
                //Stun the dog
                target.gameObject.GetComponent<Dog>().ShootTheDog();
            }
            else
            {
                target.gameObject.GetComponent<EnemyMove>().KillObject(); //Make the kill routine happen with animated bones etc
            }

        }
    }
}
