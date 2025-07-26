using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    //Called when the user clicks J, called from the PlayerShoot script
    //This script is attached the bullet itself


    private float speed = 10f;
    //private Animator anim;
    private bool canMove;
    public float direction = 1;  //Direction to face either 1 (right) or -1 (left)

    void Awake()
    {
      // anim = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(DestroyBullet(1f));
        canMove = true;

        //Set direction
        Vector3 temp = transform.localScale;
        temp.x = direction;
        transform.localScale = temp;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public float Speed //Getters and setters for the speed property of this class
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }


    void Move()
    {
        if (canMove)
        {
            //Uses the transform property to move the bullet
            //remember you need a temp you can't simply set the x property
            Vector3 temp = transform.position;

            temp.x += speed * Time.deltaTime;  //Move the bullet at the same rate as the frame moves (Time.DeltaTime)
                                               //Add the speed * the time it took to complete the last frame

            transform.position = temp;

        }

    }

    IEnumerator DestroyBullet(float timer)
    {   //Co-routine to destroy the bullet after a period of time
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D target)
    {
      
        if (target.gameObject.tag == MyTags.ENEMY_TAG || target.gameObject.tag == MyTags.PUPPY_TAG)
        {
            canMove = false;
            //anim.Play("Bullet_Animation");
            StartCoroutine(DestroyBullet(0.1f));
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
