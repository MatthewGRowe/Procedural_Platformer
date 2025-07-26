using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public float rotateSpeed = 60f;


    //Enable the speed to be accessed from elsewhere (so we can throw backwards)
    //public float Speed { get { return speed; } set { speed = value; } }
    private float direction;



    private void Start()
    {
        //Make sure bullet is destroyed
        StartCoroutine(DestroyBullet(Random.Range(1f, 1.9f)));




    }

    IEnumerator DestroyBullet(float timer)
    {
        //Destroy the Apple after a given time
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }



  
}
