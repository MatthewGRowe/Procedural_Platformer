using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeerCan : MonoBehaviour
{
    public float rotateSpeed = 60f;


    //Enable the speed to be accessed from elsewhere (so we can throw backwards)
    //public float Speed { get { return speed; } set { speed = value; } }
    public float direction;

   

    private void Start()
    {
        //Make sure bullet is destroyed
        StartCoroutine(DestroyBullet(Random.Range(2,3.5f)));
        

        

    }

    IEnumerator DestroyBullet(float timer)
    {
        //Destroy the Beer Can after a given time
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

   

    private void OnTriggerEnter2D(Collider2D target)
    {
        int randomSound = Random.Range(0, 1);
        if (randomSound == 0)
            AudioManager.instance.Play("Can1");
        else
            AudioManager.instance.Play("Can2");


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
