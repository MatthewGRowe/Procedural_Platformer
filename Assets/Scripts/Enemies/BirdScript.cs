using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour 
{

	private Rigidbody2D myBody;
	private Animator anim;

	private Vector3 moveDirection = Vector3.left;   //This is the same as putting -1 on x axis.
	private Vector3 originPosition;  				//Where we start on the screen
	private Vector3 movePosition; 					//Where we are moving to

	public GameObject birdStone;				//The object the bird will throw
	public LayerMask playerLayer;				//The layer where the player sits
	private bool attacked = false;						//Stops us attacking more than once

	private bool canMove;						//Can we move?
	private float speed;

	private int maxStones;  //Maximum stones the bird can drop
	private int stonesDroppedSoFar =0;
	private void Awake()
	{
		maxStones = Random.Range(2, 5);  //Stones to be dropped.
		myBody = GetComponent<Rigidbody2D>(); //Read in the birds body component
		anim = GetComponent<Animator>();  //Allow us to adjust the animations
	}


	// Use this for initialization
	void Start () 
	{
		originPosition = transform.position;  //Original position 
		originPosition.x += 6f;					//Set original position 6 places back (right) from where it started

		movePosition = transform.position;   //Set final position to current position
		movePosition.x -= 6f;				//Adjust final position to be 6 places forward (left) from where it started

		canMove = true;
		speed = 2.5f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		MoveTheBird ();
		DropTheStone (); //Check if we can hit the player
	}


	void MoveTheBird()
	{
		if (canMove)
		{
			transform.Translate (moveDirection * speed * Time.smoothDeltaTime); //Move the bird in the direction it needs to go smoothly
			//Translate moves in the given direction

			if (transform.position.x >= originPosition.x) //If we have reached the point on the right where we turn around
			{
				
				moveDirection = Vector3.left;
				ChangeDirection (1f);

			}
			else if (transform.position.x <= movePosition.x) //If we have reached the point on the left we turn around
			{

				moveDirection = Vector3.right;
				ChangeDirection (-1f);
			}

		}
	}

	void ChangeDirection(float direction)
	{

		Vector3 tempScale = transform.localScale; //Copy the local scale
		tempScale.x = direction;
		transform.localScale = tempScale;
	}

	void DropTheStone()
	{
		if (!attacked)
		{

			if (Physics2D.Raycast (transform.position, Vector2.down, Mathf.Infinity, playerLayer))  //Draw a raycast down, forever, search for objects on the playerlayer
			{
				if (stonesDroppedSoFar <= maxStones)
				{
					AudioManager.instance.Play(MyTags.SOUND_HAWK);
					maxStones--;
					Instantiate(birdStone, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), Quaternion.identity);
					attacked = true;
					StartCoroutine(ReStock());  //Allow bird to drop more stones
					anim.Play("BirdFly");
				}
			}
		}
	}

	IEnumerator ReStock()
	{
		//Enable bird to drop more stones
        yield return new WaitForSeconds(2f);
		anim.Play("BirdFlyLoaded");
        attacked = false;
    }
	IEnumerator BirdDead()
	{
		yield return new WaitForSeconds (3f);
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.tag == MyTags.BULLET_TAG)
		{
			anim.Play ("BirdDead");

			GetComponent<BoxCollider2D> ().isTrigger = true;  //Is Trigger will cause the falling bird to ignore barriers and fall through the ground
			myBody.bodyType = RigidbodyType2D.Dynamic; //Allow the bird to fall as Rigidbody is now Dynamic

			canMove = false;

			StartCoroutine (BirdDead ());
		}
	}
}





