using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dog : MonoBehaviour
{
	public float moveSpeed = 1;
	private Rigidbody2D myBody;
	private Vector3 offsetTransform = new Vector3();
	private Animator anim;
	private bool moveLeft;
	private bool canChangeDirection = true; //Used to delay direction change after wall turn
	private bool canDoDamage = true;
	private bool canBeKilled = false;
	private bool iAmDead = false;
	public LayerMask playerLayer;  //Allows you to choose which layer the player is on
	public LayerMask groundLayer;

	private float myoriginal_y;  //Stops snail moving up and down

	private bool canMove; //Allows the snail to stay still if it is stunned
	private bool stunned; //Indicates if the snail is stunned.

	public Transform down_Collision, top_Collision, left_Collision, right_Collision; //This is a reference to empty game objects used to check the snail and to see if it has hit anything

	//public Vector3 left_Collision_Pos, right_Collision_Pos;

	public TMP_Text myText;  //For displaying angry messages
	private List<string> angryMessages = new List<string>(); //List of angry message

	bool messageIsCleared = true;
	//Vector3 offSet = new Vector3(0, 2f, 0);


	public float fallingThreshold = 10f;
	
	public bool canFall = false;

	//You cannot kill a puppy it is very bad!!!
	//Points will be deducted for such behaviour!

	bool canBark = true;

	

	void Awake()
	{
		moveSpeed = Random.Range(1, 8);  //Move fast, move slow?
		myBody = GetComponent<Rigidbody2D>();
		offsetTransform = myText.transform.position;  //Store orignial offset
		anim = GetComponent<Animator>();
		

		angryMessages.Add("Don't hurt puppies!");
		angryMessages.Add("Brut!");
		angryMessages.Add("Beast!");
		angryMessages.Add("Ouch!");

	}


	void Start()
	{
		moveLeft = true; //Move left initially
		stunned = false;
		canMove = true;
	}


	// Update is called once per frame
	void Update()
	{
		myText.transform.position = myBody.transform.position; // + offSet;
		if (canMove)
		{
			if (moveLeft)
			{
				myBody.velocity = new Vector2(-moveSpeed, myBody.velocity.y);  //Move left
				

			}
			else
			{
				myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);  //Move right
																			  //myText.transform.position = myBody.transform.position + offsetTransform;
				

			}
		}
		CheckCollision();

		CheckIfFalling();
	}

	IEnumerator DockPoints(int pointsToDock)
    {
	
		yield return new WaitForSeconds(1f);
		
		GameManager.instance.Points(-pointsToDock);  //Dock the points
    }


	IEnumerator ReallyDead()
	{
		//Write message to info board
		GameManager.instance.InfoTextDisplay("Puppy Killer! -10 points.", 2);
		anim.SetBool("Stunned", false);
		if (iAmDead == false)
		{
			StartCoroutine(DockPoints(10));
			iAmDead = true;
		}
		
		GetComponent<CircleCollider2D>().enabled = false;
		GetComponent<Rigidbody2D>().gravityScale = -1;
		AudioManager.instance.Play(MyTags.SOUND_DOG_DIE);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);

	}



	private void CheckIfFalling()
	{
        //Is dog falling, minus numbers so 0 is not falling and -200 is falling really fast
        if (System.Math.Abs(myBody.velocity.y) < fallingThreshold && canFall)
        {
			print("Falling threshold " + myBody.velocity.y);
            Destroy(gameObject);
        }
		else if (!Physics2D.Raycast(transform.position, Vector2.down,1f) && canFall) //We are above empty space (falling)
		{
			Destroy(gameObject);
		}
        

        
        if (canBeKilled)
		{
			//canBeKilled was needed as the stunned animation was appearing in the floor
			//Box collider raised him but this then triggered this event.

			//Is there anything beneath us and are we stunned.
			if (!Physics2D.Raycast(down_Collision.position, Vector3.down, 0.1f) && stunned)
			{
				stunned = true;  //Allow pooch to move even though he is unfortunately dead
								 //We have reached the edge of a cliff

				StartCoroutine(ReallyDead());
			}
		}


	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.BULLET_TAG) //We have been shot!
		{
			ShootTheDog();
		}
    }


    void CheckCollision()
	{
		if (!Physics2D.Raycast(down_Collision.position, Vector3.down, 0.1f) && canFall)  //Cast a ray downwards from the GroundCheck object (known as down_Collision)
		{
			//No ground beneath us so change direction

			

			ChangeDirection();
		}
		else if (Physics2D.Raycast(down_Collision.position, Vector3.down, 0.1f) && canFall == false) //We are standing on the ground
		{
			canFall = true; //We have landed so we can allow the dog to fall if it is pushed off the edge.
		}



		RaycastHit2D leftHit = Physics2D.Raycast(left_Collision.position, Vector2.left, 0.1f, playerLayer);  //Search for collisions with player on the right.
		RaycastHit2D rightHit = Physics2D.Raycast(right_Collision.position, Vector2.right, 0.1f, playerLayer);  //Search for collisions with player on the right.
		Collider2D topHit = Physics2D.OverlapCircle(top_Collision.position, 0.2f, playerLayer); //Search in a circle for impacts from above


		if (topHit != null) //If we are hit from top
		{
			
			if (topHit.gameObject.tag == MyTags.PLAYER_TAG)  //If it was hit by the player
			{
				Bark();
				
				if (!stunned) //If we are not already stunned
				{
                    //Bounce the player off the snail, you can't just set the velocity remember, you need an entire vector 2
                    topHit.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(topHit.gameObject.GetComponent<Rigidbody2D>().velocity.x, 8f);

                    StunTheDog();
					

				}
			}
		}
		if (leftHit) //if we are hit from left bite the player!
		{
			if (leftHit.collider.gameObject.tag == MyTags.PLAYER_TAG)
			{
				
				Bark();
				
				if (!stunned)
				{
					if (canDoDamage)
					{
						canDoDamage = false;
						StartCoroutine(DoDamage());
					}
				}
				else
				{
					//Stunned so grunt angrily
					if (messageIsCleared)
					{

                        GameManager.instance.InfoTextDisplay(angryMessages[Random.Range(0, angryMessages.Count)],1); //Display an angry message
						messageIsCleared = false;
						StartCoroutine(ClearAngryMessage());  //Clear the angry message
					}

				}
			}
			
        }

		if (rightHit) //If we are hit from the right bite the player!
		{
			if (rightHit.collider.gameObject.tag == MyTags.PLAYER_TAG)
			{
				Bark();
			
				if (!stunned)
				{
					if (canDoDamage)
					{
						canDoDamage = false;
						StartCoroutine(DoDamage());
					}
				}
				else
				{
					if (messageIsCleared)
					{

                        GameManager.instance.InfoTextDisplay(angryMessages[Random.Range(0, angryMessages.Count)],1); //Display an angry message
						messageIsCleared = false;
						StartCoroutine(ClearAngryMessage());  //Clear the angry message
					}
				}
			}
           
        }

		if (canChangeDirection) //Don't bother checking unless we can actually change direction (short delay after colliding with a wall)
		{
			RaycastHit2D leftWallHit = Physics2D.Raycast(left_Collision.position, Vector2.left, 0.1f, groundLayer);  //Search for collisions with player on the right.
			RaycastHit2D rightWallHit = Physics2D.Raycast(right_Collision.position, Vector2.right, 0.1f, groundLayer);  //Search for collisions with player on the right.

			if (leftWallHit || rightWallHit)
			{
				canChangeDirection = false;
				ChangeDirection();  //Change direction if you hit a wall.
				StartCoroutine(AllowDirectionChangeAfterHittingAWall());
			}
		}

    }

	IEnumerator AllowDirectionChangeAfterHittingAWall()
	{
		yield return new WaitForSeconds(1.5f);
		canChangeDirection = true;
	}

    private void StunTheDog()
    {
        //FindObjectOfType<AudioManager>().Play(MyTags.SOUND_JUMP);
        

        canMove = false;  //We are now stunned
        myBody.velocity = new Vector2(0, 0);  //Stop from moving

        anim.SetBool("Stunned", true);  //Change to the stunned animation
        GetComponent<CircleCollider2D>().offset = new Vector2(GetComponent<CircleCollider2D>().offset.x, -0.6f);
        GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<CircleCollider2D>().offset.x, -0.6f);

        float downx = down_Collision.position.x;
        float downy = down_Collision.position.y;


        down_Collision.localPosition = new Vector3(0, -1.2f, 0); //Reset into floor
        stunned = true;
        StartCoroutine(AllowedToBeKilled());

        if (messageIsCleared)
        {

            GameManager.instance.InfoTextDisplay(angryMessages[Random.Range(0, angryMessages.Count)], 1); //Display an angry message
            messageIsCleared = false;
            StartCoroutine(ClearAngryMessage());  //Clear the angry message
        }
    }

    public void ShootTheDog()
	{
        GameManager.instance.InfoTextDisplay("DON'T SHOOT PUPPIES!", 2); //Display an angry message
		messageIsCleared = false; 
		StartCoroutine(ClearAngryMessage());
		GameManager.instance.Points(-2);
        if (!stunned)
		{
			StunTheDog();
		}
	}

	void Bark()
    {
		if (canBark)
		{
			StartCoroutine(DockPoints(2));
			AudioManager.instance.Play(MyTags.SOUND_WOOF);
			StartCoroutine(BarkPause());
		}

	}

	IEnumerator AllowedToBeKilled()
    {
		yield return new WaitForSeconds(2f);
		canBeKilled = true;
    }
	IEnumerator DoDamage()
    {
		GameManager.instance.DropHealth(0.2f);
		yield return new WaitForSeconds(2);
		canDoDamage=true;

    }
	IEnumerator BarkPause()
    {
		canBark = false;
		yield return new WaitForSeconds(1);
		canBark = true;
    }
	void ChangeDirection()
	{
		canFall = true;  //Enable the dog to fall after the first direciton change (otherwise it dies mid-air before the game starts!)
		//Swap the direction
		moveLeft = !moveLeft;

		Vector3 tempScale = transform.localScale; //Copy the x,y,z co-ordinates of the snail
	

		if (moveLeft)  //If we are now moving left
		{
			tempScale.x = Mathf.Abs(tempScale.x);  //Make sure the x axis has a positive value so whatever it was will be swapped to a positive
			//myText.rectTransform.localScale = new Vector3(1, 1);   //Keep text in the same direction



		}
		else
		{
			tempScale.x = -Mathf.Abs(tempScale.x);  //Make sure the x axis has a negative value so whatever it was will be swapped to a negative
			//myText.rectTransform.localScale = new Vector3(-1, 1);   //Keep text in the same direction

		}

		transform.localScale = tempScale;
		
		//myText.rectTransform.localScale = new Vector3(1, 1, 1);   //Keep text in the same direction

	}



	/*IEnumerator Dead(float timer)
	{

		yield return new WaitForSeconds(timer);
		Destroy(gameObject);
	}*/

	IEnumerator ClearAngryMessage()
	{

		yield return new WaitForSeconds(3f);
		myText.text = "";  //Blank the message
		messageIsCleared=true;
	}

}
