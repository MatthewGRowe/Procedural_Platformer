using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.Rendering.Universal;

public class checkPoint : MonoBehaviour
{
    public Sprite[] onOffImages = new Sprite[2];  //0 = off 1 = on
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Light2D myLight;
    private Rigidbody2D myBody;
    public int myNumber;
 

    private GameObject blocker; //Blocks player from going backwards once a checkpoint is passed
    
    public TMP_Text myText;  //Reads checkpoint x of y

    private void Start()
    {
        myBody = GetComponent<Rigidbody2D>();

        

        //Blocker is a massive box collider that stops the player walking backwards
        blocker = transform.Find("Blocker").gameObject;
        blocker.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        if (collision.tag == MyTags.PLAYER_TAG && myLight.enabled == false)
        {
            blocker.SetActive(true); //Stop player walking backwards
            AudioManager.instance.Play(MyTags.SOUND_BONUS);
            GetComponent<SpriteRenderer>().sprite = onOffImages[1];  //Change to ON
            GameManager.instance.latestCheckPoint = transform.position;  //Record our location
            print("Checkpoint recorded at " + transform.position.x + "," + transform.position.y);
            myLight.enabled = true;

            //Flash the checkpoint number for 2 seconds
            //TODO: Check if the total should come from the game manager?
            myText.text = myNumber + " of " + GameManager.instance.totalCheckPoints;  //Set text
            StartCoroutine(TurnOffText()); //Turn it off

        }
        else if (collision.tag == MyTags.GROUND_LAYER_TAG)
        {

            myBody.bodyType = RigidbodyType2D.Static;
            GetComponent<CircleCollider2D>().enabled = false;
        }
        

    }

    IEnumerator TurnOffText()
    {
        yield return new WaitForSeconds(2);
        myText.text = "";

    }
   

 

}
