using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 1f;
    public GameObject proximitySensor;

    private Rigidbody2D myBody;
    private Animator anim;
    private bool moveLeft;
    private bool iCanKillHim = true;

    public LayerMask playerLayer;  //Allows you to choose which layer the player is on



    private bool imAlive, reallyReallyDead; //Allows the snail to stay still if it is stunned


    public Transform down_Collision, top_Collision, left_Collision, right_Collision; //This is a reference to empty game objects used to check the snail and to see if it has hit anything




    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }


    void Start()
    {
        moveLeft = true; //Move left initially

        imAlive = true;
        reallyReallyDead = false; //Needed or you get loads of dead sounds

    }


    // Update is called once per frame
    void Update()
    {


        if (imAlive)
        {
            if (moveLeft)
            {
                myBody.velocity = new Vector2(-moveSpeed, myBody.velocity.y);  //Move left
            }
            else
            {
                myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);  //Move right
            }

            CheckCollision();
            //Move proximity sensor
            proximitySensor.transform.position = transform.position;
        }
        else
        {
            CheckIfFalling();  //Player may have pushed us off a cliff!
        }
    }

    IEnumerator ReallyDead()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().drag = 5;
        AudioManager.instance.Play(MyTags.SOUND_SKELETONDIE);
        GameManager.instance.Points(5);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);

    }

    private void CheckIfFalling()
    {

        if (!Physics2D.Raycast(down_Collision.position, Vector3.down, 0.1f) && imAlive == false && reallyReallyDead == false)
        {

            //We have reached the edge of a cliff
            reallyReallyDead = true;
            StartCoroutine(ReallyDead());
        }


    }

    void CheckCollision()
    {

        if (!Physics2D.Raycast(down_Collision.position, Vector3.down, 0.1f))  //Cast a ray downwards from the GroundCheck object (known as down_Collision)
        {
            //No ground beneath us so change direction

            ChangeDirection();
        }



        RaycastHit2D leftHit = Physics2D.Raycast(left_Collision.position, Vector2.left, 0.1f, playerLayer);  //Search for collisions with player on the right.
        RaycastHit2D rightHit = Physics2D.Raycast(right_Collision.position, Vector2.right, 0.1f, playerLayer);  //Search for collisions with player on the right.
        Collider2D topHit = Physics2D.OverlapCircle(top_Collision.position, 0.3f, playerLayer); //Search in a circle for impacts from above


        if (topHit != null) //If skeleton is hit from top
        {
            if (topHit.gameObject.tag == MyTags.PLAYER_TAG)  //If it was hit by the player
            {
                //Bounce the player off the snail, you can't just set the velocity remember, you need an entire vector 2
                if (topHit.GetComponent<Rigidbody2D>() != null)  //Caveman has rigidbody in parent
                {
                    topHit.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(topHit.gameObject.GetComponent<Rigidbody2D>().velocity.x, 4f);
                }
                else
                {
                    topHit.gameObject.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(topHit.gameObject.GetComponent<Rigidbody2D>().velocity.x, 4f);
                }
                //Run kill routine
                KillObject();
            }
        }
        if (leftHit  || rightHit) //if we are hit from left
        {
            string collisionTag = "";
            //Read in the tag
            if (leftHit)
            {
                collisionTag = leftHit.collider.tag;
            }
            else
            {
                collisionTag = rightHit.collider.tag;   
            }

            if (collisionTag == MyTags.PLAYER_TAG )
            {   //We have been hit by the player!
                if (GameManager.instance.playerInvincible)  //We have hit the skelleton or 
                {
                    
                    //Run kill routine to kill this enemy
                    KillObject();
                }
                else if (iCanKillHim)  //Kill the player
                {
                    iCanKillHim = false;
                    StartCoroutine(KillPlayer());
                }

            }
        }

       

    }

    public void KillObject()
    {
        GameManager.instance.InfoTextDisplay("Killing Skelleton", 2);
        //Adust collider so we don't float
        //Vector2 tempOffset = GetComponent<CapsuleCollider2D>().offset;
        Vector2 tempSize = GetComponent<CapsuleCollider2D>().size;

        //tempOffset.y = -0.85f;
        tempSize.y = 0.9f;

        //GetComponent<CapsuleCollider2D>().offset = tempOffset; //Move collider
        GetComponent<CapsuleCollider2D>().size = tempSize;
                                                                          //Stop Proximity Noise
        proximitySensor.GetComponent<proximityNoise>().StopNoise();

        Destroy(proximitySensor.gameObject);
        GameManager.instance.Points(10);
        AudioManager.instance.Play(MyTags.SOUND_HEAVYTHROW);
        anim.SetBool("dead", true);

        
        //Make bones heavy
        GetComponent<Rigidbody2D>().mass = 5;
        GetComponent<Rigidbody2D>().drag = 10;

        imAlive = false;
    }

    IEnumerator KillPlayer()
    {
        GameManager.instance.LoseALife();   //Search for player damage script and run dealdamage method
        yield return new WaitForSeconds(2);
        iCanKillHim = true;
    }

    void ChangeDirection()
    {
        //Swap the direction
        moveLeft = !moveLeft;

        Vector3 tempScale = transform.localScale; //Copy the x,y,z co-ordinates of the snail

        if (tempScale.x < 0)
        {
            tempScale.x = Mathf.Abs(tempScale.x);  //Make sure the x axis has a positive value so whatever it was will be swapped to a positive
        }
        else
        {
            tempScale.x = -Mathf.Abs(tempScale.x);  //Make sure the x axis has a negative value so whatever it was will be swapped to a negative
        }



        transform.localScale = tempScale;
    }

    void OnTriggerEnter2D(Collider2D trigger)  //Used for bullet collisions
    {
        
        if (imAlive)
        {
            if (trigger.tag == MyTags.BULLET_TAG)
            {
                KillObject(); //Kill the skelleton 
                
            }
            else if (trigger.tag == MyTags.PLAYER_TAG)
            {
                GameManager.instance.LoseALife();
            }
        }
        

    }



}