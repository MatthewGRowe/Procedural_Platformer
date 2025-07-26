using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdStone : MonoBehaviour 
{

	private bool canDamage = true;
	private bool grounded = false;
	private bool readyForTakeOff = false;
	private bool flying = false;
	Rigidbody2D myBody;
	Animator myAnim;
	private int speed = 8;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();	
		myAnim = GetComponent<Animator>();
    }


    void OnCollisionEnter2D(Collision2D target)
	{
		//Only used when using colliders without a trigger.
		//Solid colliders!
		
		if (target.gameObject.tag == MyTags.PLAYER_TAG) //IF we have hit the player
		{
			if (canDamage  || flying)
			{
				canDamage = false;  //Make sure it only happens once!
									//Damage player
				GameManager.instance.DropHealth(0.3f);

			}
            else if (grounded) //We have landed on the floor
            {
				//Player has collided with the egg so it is time to hatch!
				//HatchAnimation
				myAnim.SetBool("hatch", true);
                AudioManager.instance.Play(MyTags.SOUND_BABYBIRD);
            }
        }
		
		else if (target.gameObject.tag == MyTags.GROUND_LAYER_TAG && readyForTakeOff == false) //We are on the ground
		{
			myBody.bodyType = RigidbodyType2D.Static;
			grounded = true;
		}
		
	}

	public void StartFly()
	{
		
		if (flying == false)
		{
			flying = true;
            //Delay the takeoff by 1 second
            StartCoroutine(SpreadWings());
        }
		

	}

	IEnumerator SpreadWings()
	{
		yield return new WaitForSeconds(1);
		readyForTakeOff = true;
        myBody.bodyType = RigidbodyType2D.Kinematic;
        //calculate destination
        float x = transform.position.x - 30;  //Left
		float y = transform.position.y + 7; //Above
		Vector3 destination = new Vector3(x, y, 0);
		print("Flapping towards " + x + "," + y);
        while (transform.position != destination)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            yield return null;
        }
		Destroy(gameObject);

    }

   


}
