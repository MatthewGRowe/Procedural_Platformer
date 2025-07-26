using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine;
using Unity.Burst.CompilerServices;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 5f;
    private Rigidbody2D myBody;
    private Animator anim;
    public static PlayerMovement instance;

    public Transform groundCheckPosition; //This is an empty game object below the player's feet
    public LayerMask groundLayer;  //Allows you to state that you are only 
                                   //Checking for collisions on a particular layer

    private bool isGrounded;  //Is our player on the ground?
    private bool jumped;  //Has our player jumped?
    public bool isFacingRight;

    public float fallMultiplier = 2.5f;
    public float jumpPower = 9f;  //How much lift are we going to provide in a jump

    public bool levelReset = true;
    public bool falling = false;
    private float fallingThreshold = 13;
    private float lastY = 0;

    void Update()
    {
        CheckIfGrounded();  //See if we are on the ground and set variables accordingly
        if (!jumped && Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }

        
        CheckIfFalling();
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

    // Use this for initialization
    void Awake()
    {


        myBody = GetComponent<Rigidbody2D>(); //Get the RigidBody component from the player
        anim = GetComponent<Animator>(); //Link to the animator component
        lastY = transform.position.y; //Check for a fall!

        

    }

    private void Start()
    {
        //Set player up as it is not persistant accross screens
        GameManager.instance.ResetPlayerStats();  //Set up new player stats in case player has died
        GameManager.instance.player = this.gameObject;
        GameManager.instance.jumpPower = jumpPower;
        GameManager.instance.originalJumpPower = jumpPower;
        GameManager.instance.latestCheckPoint = transform.position;
        transform.position = GameManager.instance.latestCheckPoint;

        if (gameObject.name.Contains("Hero"))
        {
            //Hero needs bullets and bigger jump
            GameManager.instance.Ammo(5);
            jumpPower += 5;
            GameManager.instance.jumpPower = jumpPower;
            GameManager.instance.originalJumpPower = jumpPower;
        }

        
    }

    void FixedUpdate()
    {

        PlayerWalk();
    }


    void PlayerWalk()
    {
        //Called to allow the player to walk.
        //Notice no need to read keys!!!!!!!!!!

        float h = Input.GetAxisRaw("Horizontal"); //AxisRaw = 1 or -1 rather than Axis which is a number up to 1 or -1
        if (myBody.bodyType == RigidbodyType2D.Dynamic)  //Do not run if we are falling or losing a life 
        {
            if (h > 0)
            {
                isFacingRight = true;
                myBody.velocity = new Vector2(speed, myBody.velocity.y);
                ChangeDirection(1); //Send direction to face
                GameManager.instance.playerIsIdle = false;  //Needed so that we can stop parralax particles moving
            }
            else if (h < 0)
            {
                isFacingRight = false;
                myBody.velocity = new Vector2(-speed, myBody.velocity.y);
                ChangeDirection(-1); //Send direction to face
                GameManager.instance.playerIsIdle = false;  //Needed so that we can stop parralax particles moving
            }
            else if (h == 0)
            {
                myBody.velocity = new Vector2(0f, myBody.velocity.y);
                GameManager.instance.playerIsIdle = true;  //Needed so that we can stop parralax particles moving
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
        Vector3 tempScale = transform.localScale; //Copies the local x,y,z values into temp variable
        tempScale.x = direction; //Alter direction
        transform.localScale = tempScale; //Overwrite the local values
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
                jumped = false; //If we have jumped before then unset the jumped variable
                myBody.gravityScale = 1;
                anim.SetBool("Jump", false); //Turn off the jump animation
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
       jumped = true;
       AudioManager.instance.Play(MyTags.SOUND_JUMP);
       
       //myBody.AddForce(Vector2.up , ForceMode2D.Impulse);
       myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);  //Adjust the y so we jump at the set power
       anim.SetBool("Jump", true); //Update the animator so we change to a jump sprite
       
    }
}
