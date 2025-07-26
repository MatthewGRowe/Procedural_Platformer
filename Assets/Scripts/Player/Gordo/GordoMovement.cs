using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class GordoMovement : MonoBehaviour
{

    public float speed = 5f;
    private Rigidbody2D myBody;

    public static PlayerMovement instance;

    public Transform groundCheckPosition; //This is an empty game object below the player's feet
    public LayerMask groundLayer;  //Allows you to state that you are only 
                                   //Checking for collisions on a particular layer

    private bool isGrounded;  //Is our player on the ground?
    private bool jumped;  //Has our player jumped?
    private Animator anim;

    public float fallMultiplier = 2.5f;
    public float jumpPower = 9f;  //How much lift are we going to provide in a jump

    public bool levelReset = true;
    public bool falling = false;
    private float fallingThreshold = 13;
    private float lastY = 0;

    //Shooting
    public GameObject fireBullet;  //The bullet prefab 
    public Transform bulletInstantiationPoint;
    public bool canShoot = true;
    public Sprite myAmmoSprite;
    [SerializeField] //Serialized as it does not need to be called anywhere else but I want to alter in in the inspector
    private float throwDelay = 1;
    [SerializeField] //Serialized as it does not need to be called anywhere else but I want to alter in in the inspector

    public float horizontalSpeed = 10f;  //These enable you to control the trajectory of the throw.
    public float verticalSpeed = 3f;

    ////[Header("Debug Beer Can")]
    //public float xangularDrag = 0.05f;
    //public float xmass = 1f;
    //public float xlinearDrag = 0f;
    //public float xgravityScale = 1f;
    

    //Animation Styles
    const string animWalk = "Walk";
    const string animRun1 = "Run_01";
    const string animRun2 = "Run_02";
    const string animShoot = "Throw";
    const string animJump = "Jump";
    const string animIdle = "Idle";

    //Animation Current State (meaning you don't need the animator spider web!)
    private string currentState;

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
        GameManager.instance.Ammo(10);  //Give 10 cans to start
        GameManager.instance.player = this.gameObject;
        GameManager.instance.jumpPower = jumpPower;
        GameManager.instance.originalJumpPower = jumpPower;
        GameManager.instance.latestCheckPoint = transform.position;
        GameManager.instance.AmmoSprite = myAmmoSprite;  //Set sprite
        transform.position = GameManager.instance.latestCheckPoint;
    }

    void Update()
    {
        //Update is used for input checks, FixedUpdate is used for executing code.
        
        CheckIfGrounded();  //See if we are on the ground and set variables accordingly
        
    }

    private void FixedUpdate()
    {
        //Update is used for input checks, FixedUpdate is used for executing code.
        PlayerWalk();
        if (!jumped && Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }

        ThrowCan();
        CheckIfFalling();
    }

    void ChangeAnimationState(string newState)
    {
        //Stop the same animation from interrupting itself
        if (currentState == newState) return;

        //play the animation
        anim.Play(newState);

        //Reassign the current state
        currentState = newState;
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


    void PlayerWalk()
    {
        //Called to allow the player to walk.
        //Notice no need to read keys!!!!!!!!!!
   
        float h = Input.GetAxisRaw("Horizontal"); //AxisRaw = 1 or -1 rather than Axis which is a number up to 1 or -1
        if (myBody.bodyType == RigidbodyType2D.Dynamic)  //Do not run if we are falling or losing a life 
        {

            if (canShoot)  //Only alow these to play if the player is not shooting
            {
                if (h > 0)
                {
                    myBody.velocity = new Vector2(speed, myBody.velocity.y);
                    ChangeDirection(-0.3f); //Send direction to face
                    GameManager.instance.playerIsIdle = false;  //Needed so that we can stop parralax particles moving

                }
                else if (h < 0)
                {
                    myBody.velocity = new Vector2(-speed, myBody.velocity.y);
                    ChangeDirection(0.3f); //Send direction to face
                    GameManager.instance.playerIsIdle = false;  //Needed so that we can stop parralax particles moving

                }
                else if (h == 0)
                {
                    myBody.velocity = new Vector2(0f, myBody.velocity.y);
                    GameManager.instance.playerIsIdle = true;  //Needed so that we can stop parralax particles moving
                }
                if (isGrounded && canShoot) //Only do this if we are on the ground and not shooting
                {
                    if (h != 0)
                    {
                        ChangeAnimationState(animRun1);
                    }
                    else
                    {
                        ChangeAnimationState(animIdle);
                    }
                }
            }
        }

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

    void ChangeDirection(float direction)  //Change the direction the character is facing
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
        print("Grounded " + isGrounded);

        if (isGrounded)
        {

            if (jumped) //If we have already jumped
            {
                jumped = false; //If we have jumped before then unset the jumped variable
                myBody.gravityScale = 1;
                anim.SetBool("Jump", false); //Turn off the jump animation
                ChangeAnimationState (animRun2);
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
        ChangeAnimationState(animJump); //Update the animator so we change to a jump sprite

    }

    void ThrowCan()
    {
       
        if (GameManager.instance.GetBullets() > 0)
        {

            if (Input.GetKeyDown(KeyCode.J) && canShoot) //If user has pressed "J"
            {
                
                canShoot = false;
                

                ChangeAnimationState(animShoot);

               
               


                StartCoroutine(EnableShoot());
            }
           

        }
    }

    public void StopThrow()
    {
       
        

        //What animation to run when shoot is over
        if (isGrounded)
        {
            ChangeAnimationState(animJump);
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                ChangeAnimationState(animIdle);
            }
            else
            {
                ChangeAnimationState(animRun1);
            }
        }
        
        
    }

    IEnumerator EnableShoot()
    {
        yield return new WaitForSeconds(throwDelay);
        GameObject bullet = Instantiate(fireBullet, bulletInstantiationPoint.position, Quaternion.identity); //Create an instance / copy of the bullet
                                                                                                             //transform.position = the co-ords of the player as this script is attached to the player
                                                                                                             //Quaternion identity is a shortcut for 0,0,0

        //Rigidbody2D bRB = bullet.GetComponent<Rigidbody2D>();

        ////Debug test
        //bRB.angularDrag = xangularDrag;
        //bRB.mass = xmass;
        //bRB.drag = xlinearDrag;
        //bRB.gravityScale = xgravityScale;

        
        
        Vector2 throwVelocity = new Vector2(horizontalSpeed, verticalSpeed);

        if (transform.localScale.x >= 0)  //Reverse the direction if player is facing left
        {
            throwVelocity = new Vector2(-horizontalSpeed, verticalSpeed);
        }

        bullet.gameObject.GetComponent<Rigidbody2D>().velocity = throwVelocity;

        //Throw beer can at a jaunty rotation
        Vector3 rotation = new Vector3(0, 0, Random.Range(-50f, 50f));
        bullet.transform.eulerAngles = rotation;


        //bullet.GetComponent<BeerCan>().direction = transform.localScale.x;
        GameManager.instance.Ammo(1); //Reduce ammo
                                      //This sets the bullet speed to either +10 or -10
                                      //depending on the direction the player is facing
                                      //The Speed property is a public property of the FireBullet class (script)
   
        canShoot = true;
    }

    
}
