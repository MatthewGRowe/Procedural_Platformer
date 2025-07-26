using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformBossMove : MonoBehaviour
{
    [Tooltip("Assign your Ground Tilemap here")]
    public Tilemap groundTilemap;
    [Tooltip("Radius in world units")]
    public float blastRadius = 2f;

    public float moveSpeed = 1f;
    
    private Rigidbody2D myBody;
    private Animator anim;
    private bool moveLeft;
    

    public LayerMask playerLayer;  //Allows you to choose which layer the player is on

    private bool imAlive = true;
    private bool stopSpammingDamage = false;

    public Transform down_Collision, top_Collision, left_Collision, right_Collision; //This is a reference to empty game objects used to check the snail and to see if it has hit anything

    private int myHealth = 100;


    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }


    void Start()
    {
        moveLeft = true; //Move left initially

        imAlive = true;


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


        if (topHit != null) //If enemy hit from top
        {
            if (topHit.gameObject.tag == MyTags.PLAYER_TAG)  //If it was hit by the player
            {
                //Bounce the player off the head, you can't just set the velocity remember, you need an entire vector 2
                if (topHit.GetComponent<Rigidbody2D>() != null)  //Caveman has rigidbody in parent
                {
                    topHit.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(topHit.gameObject.GetComponent<Rigidbody2D>().velocity.x, 4f);
                    //Run kill routine
                    KillObject();
                }
                else  //Bounce anything off the head
                {
                    topHit.gameObject.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(topHit.gameObject.GetComponent<Rigidbody2D>().velocity.x, 4f);
                }
                
            }
        }
        if (leftHit || rightHit) //if we are hit from left
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

            if (collisionTag == MyTags.PLAYER_TAG && stopSpammingDamage == false)
            {
                stopSpammingDamage = true;
               //We have been hit by the player!
                GameManager.instance.DropHealth(0.1f);  //Injure the player
                AudioManager.instance.Play(MyTags.SOUND_WOOF, 0.4f);  //Ouch (sort of!)
                StartCoroutine(AllowDamage());
            }
        }



    }

    IEnumerator AllowDamage()
    {
        yield return new WaitForSeconds(0.8f);
        stopSpammingDamage = false;

    }

    private void TakeDamage()
    {
        //We need 5 hits to die (because we iz 'ard!)
        myHealth -= 20; //Reduce health by 20
        if (myHealth <= 0)
        {
            KillObject();
        }
    }

    public void KillObject()
    {

        if (!imAlive) return; // prevent repeat calls

        imAlive = false;
        GameManager.instance.InfoTextDisplay("Killing Locum!", 2);

        // Adjust collider to fit collapsed body
        Vector2 tempSize = GetComponent<CapsuleCollider2D>().size;
        tempSize.y = 0.9f;
        GetComponent<CapsuleCollider2D>().size = tempSize;

        // Stop proximity noise
        Transform proximitySensor = transform.Find("ProximitySensor");
        if (proximitySensor != null)
        {
            proximitySensor.GetComponent<proximityNoise>().StopNoise();
            Destroy(proximitySensor.gameObject);
        }

        GameManager.instance.Points(50);
        anim.SetBool("Die", true);
        GetComponent<Rigidbody2D>().mass = 100;

        // Wait before exploding and destroying
        StartCoroutine(HandleDeathSequence());
    }

    IEnumerator HandleDeathSequence()
    {
        // Wait until the "Die" animation has played fully
        float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        Explode();
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
                TakeDamage(); //Need 5 hits 

            }
            else if (trigger.tag == MyTags.PLAYER_TAG && stopSpammingDamage == false)
            {
                stopSpammingDamage = true;
                GameManager.instance.DropHealth(0.1f);
                StartCoroutine(AllowDamage());
            }
        }


    }

    public void Explode()
    {
        Vector3 worldPos = transform.position;
        RemoveTilesAround(worldPos, blastRadius);
        AudioManager.instance.Play(MyTags.SOUND_EXPLOSION);
        Destroy(gameObject);
    }

    void RemoveTilesAround(Vector3 centerWorldPos, float radius)
    {
        // Convert world-space center to cell position
        Vector3Int centerCell = groundTilemap.WorldToCell(centerWorldPos);

        // Compute radius in cells (approx)
        int cellRadius = Mathf.CeilToInt(radius / groundTilemap.cellSize.x);

        for (int dx = -cellRadius; dx <= cellRadius; dx++)
        {
            for (int dy = -cellRadius; dy <= cellRadius; dy++)
            {
                Vector3Int cellPos = new Vector3Int(centerCell.x + dx, centerCell.y + dy, centerCell.z);
                // Check actual distance so it’s circular
                Vector3 cellCenterWorld = groundTilemap.GetCellCenterWorld(cellPos);
                if (Vector3.Distance(cellCenterWorld, centerWorldPos) <= radius)
                {
                    // Clear that tile
                    groundTilemap.SetTile(cellPos, null);
                }
            }
        }
    }

}
