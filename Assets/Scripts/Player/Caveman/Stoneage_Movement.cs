using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stoneage_Movement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D myBody;
    public Animator myAnim;
    public static PlayerMovement instance;

    public Transform groundCheckPosition; //This is an empty game object below the player's feet
    public LayerMask groundLayer;  //Allows you to state that you are only 
                                   //Checking for collisions on a particular layer
    public LayerMask waterLayer;

    private bool isGrounded;  //Is our player on the ground?
    private bool jumped;  //Has our player jumped?


    public float fallMultiplier = 2.5f;
    public float jumpPower = 9f;  //How much lift are we going to provide in a jump

    public bool levelReset = true;
    public bool falling = false;
    private float fallingThreshold = 13;
    private float lastY = 0;

    private List<Collider2D> disabledColliders = new List<Collider2D>();

    private Stoneage_Shoot myShootScript;  //Ref to the shoot script

    // Use this for initialization
    void Awake()
    {
        myBody = GetComponentInParent<Rigidbody2D>(); //Get the RigidBody component from the player
        myAnim = GetComponent<Animator>(); //Link to the animator component
        lastY = transform.position.y; //Check for a fall!

        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.m_Follow = this.transform;

        myShootScript = GetComponent<Stoneage_Shoot>(); //Read in the shoot script
    }

    private void Start()
    {
        //Set player up as it is not persistant accross screens
        GameManager.instance.ResetPlayerStats();  //Set up new player stats in case player has died
        //GameManager.instance.player = this.gameObject;
        GameManager.instance.jumpPower = jumpPower;
        GameManager.instance.originalJumpPower = jumpPower;
        GameManager.instance.latestCheckPoint = transform.position;
        transform.position = GameManager.instance.latestCheckPoint;

        ////Improve jump power
        //GameManager.instance.Ammo(5);
        //jumpPower += 5;
        //GameManager.instance.jumpPower = jumpPower;
        //GameManager.instance.originalJumpPower = jumpPower;



    }


    void Update()
    {
        CheckIfGrounded();  //See if we are on the ground and set variables accordingly
        PlayerWalk();


        if (!jumped && Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }


        CheckIfFalling();
        
    }

    private void ResetAnimatorStates()
    {
        myAnim.SetBool("walk", false);
        myAnim.SetBool("jump", false);
        myAnim.SetBool("attack", false);
        myAnim.SetBool("idle", true);  // Ensure idle is the default state


    }

    



    // Coroutine to re-enable colliders after delay
    private IEnumerator RestoreColliders()
    {
        yield return new WaitForSeconds(2); // Wait for specified time

        foreach (Collider2D col in disabledColliders)
        {
            if (col != null) // Check if the object still exists
            {
                col.enabled = true; // Re-enable collider
            }
        }
        disabledColliders.Clear(); // Clear the list
    }

    private void CheckIfFalling()
    {
        //Is player falling, minus numbers so 0 is not falling and -200 is falling really fast
        if (myBody.velocity.y < -fallingThreshold)
        {
            falling = true;
        }
        else
        {
            falling = false;
        }

        if (falling && myBody.position.y < -8 && levelReset == true)  //Stop falling before game starts
        {
            ResetAnimatorStates();
           
            levelReset = false;  //Reset by game manager

            //print("Falling! ");
            GameManager.instance.LoseALife();

        }

        if (falling && myBody.position.y < -400f)
        {
            //For some reason the level reset has not kicked in
            levelReset = true;
        }
    }

    
    


    void PlayerWalk()
    {
        
        if (myShootScript != null && myShootScript.IsAttacking)
        {
            //Only run the rest of this if we are not attacking
            return;
        }
        //Called to allow the player to walk.
        float h = Input.GetAxisRaw("Horizontal"); //AxisRaw = 1 or -1 rather than Axis which is a number up to 1 or -1
        if (myBody.bodyType == RigidbodyType2D.Dynamic)  //Do not run if we are falling or losing a life 
        {
            if (isGrounded) //We are on the ground
            {
                ResetAnimatorStates();
                if (h > 0)
                {
                    myAnim.SetBool("walk", true);
                    myBody.velocity = new Vector2(speed, myBody.velocity.y);
                    ChangeDirection(1); //Send direction to face
                    GameManager.instance.playerIsIdle = false; //Needed so that we can stop parralax particles moving
                }
                else if (h < 0)
                {
                    myAnim.SetBool("walk", true);
                    myBody.velocity = new Vector2(-speed, myBody.velocity.y);
                    ChangeDirection(-1); //Send direction to face
                    GameManager.instance.playerIsIdle = false; //Needed so that we can stop parralax particles moving
                }
                else if (h == 0)
                {
                    myAnim.SetBool("idle", true);
                    myBody.velocity = new Vector2(0f, myBody.velocity.y);
                    GameManager.instance.playerIsIdle = true; //Needed so that we can stop parralax particles moving
                }
            }
            else if (jumped && h != 0) //If player is mid-jump allow him to change direction
            {                   //but keep jump animation
                
                if (h > 0)
                {
                    
                    myBody.velocity = new Vector2(speed, myBody.velocity.y);
                    ChangeDirection(1); //Send direction to face
                    GameManager.instance.playerIsIdle = false; //Needed so that we can stop parralax particles moving
                }
                else if (h < 0)
                {
                    
                    myBody.velocity = new Vector2(-speed, myBody.velocity.y);
                    ChangeDirection(-1); //Send direction to face
                    GameManager.instance.playerIsIdle = false; //Needed so that we can stop parralax particles moving
                }
            }
        }
        //anim.SetInteger("Speed", Mathf.Abs((int)myBody.velocity.x)); //Set speed in the animator so we can apply the appropriate animation

    }

    public void setJumpPower(float newPower, string sender)
    {
        //Only to be called by GameManager, so sender must equal GameManager!!

        if (sender == "GameManager")
        {
            jumpPower = newPower;
        }
        else
        {
            print("Error, cannot call this from anywhere but game manager!");
        }

    }

    void ChangeDirection(int direction)  //Change the direction the character is facing
    {
        if (transform.parent.name == "Policeman(Clone)")
        {
            Vector3 tempScale = transform.parent.localScale; // Get parent's scale
            tempScale.x = direction; // Alter direction
            transform.parent.localScale = tempScale; // Apply the change to the parent

        }
        else //Not changed for caveman (yet!)
        {
            Vector3 tempScale = transform.localScale; //Copies the local x,y,z values into temp variable
            tempScale.x = direction; //Alter direction
            transform.localScale = tempScale; //Overwrite the local values

        }
    }

  
                
           

    void CheckIfGrounded()
    {
        //Cast a ray from the player's feet down to see if we are on the ground (note: only checking on the groundLayer)
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector2.down, 0.1f, groundLayer);

        // Set the boolean like before (checks if we hit anything)
        isGrounded = groundHit.collider != null;
      
        if (isGrounded)
        {

            if (jumped) //If we have already jumped
            {
                GameManager.instance.playerJumping = false; //Tell the gameManager
                jumped = false; //If we have jumped before then unset the jumped variable
                myBody.gravityScale = 1;

                myAnim.SetBool("jump", false);
                myAnim.SetBool("idle", true);  // Ensure idle is set
            }

            //Turn off the noJump script if we are the upper platforms (it causes problems and won't automatically foisllow the player up a ladder)
            // Check the tag of the object we're standing on
            string groundTag = groundHit.collider.tag;

            // Example: Do something special if standing on an "Ice" surface
            if (groundTag == MyTags.SECRET_TAG)
            {
                GameManager.instance.onStairs = true; //This value will turn off the CameraNoJump script
            }
            else
            {
                GameManager.instance.onStairs = false; //This value will turn on the CameraNoJumpScript
            }
        }


    }





    void PlayerJump()
    {
        GameManager.instance.playerJumping = true; //Tell the gameManager
        jumped = true;
        AudioManager.instance.Play(MyTags.SOUND_JUMP);

        //myBody.AddForce(Vector2.up , ForceMode2D.Impulse);
        if (myBody.bodyType == RigidbodyType2D.Dynamic) //Make sure player is not being killed
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);  //Adjust the y so we jump at the set power
        }
        myAnim.SetBool("walk", false);
        myAnim.SetBool("idle", false);
        myAnim.SetBool("jump", true); //Update the animator so we change to a jump sprite

    }
}
