using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class AppleBasket : MonoBehaviour
{
    //Give a portion of apples
    //Disapear for a length of time
    //Display apples on Canvas

    
    private TextMeshProUGUI txtApples; //Used to display the number of apples
    
    public bool basketRespawnning = false;  //Public as it can be set from AppleToss.cs

    private void Start()
    {
        basketRespawnning = false;
    }

    private void Update()
    {
        
        if (GameManager.instance.apples == 0 && basketRespawnning == false)
        {
            
            basketRespawnning=true;
            txtApples.text = "0 apples \u263B\u263B, more are coming, be patient.(sic) \u263B";
            int respawnTime = Random.Range(10, 15);  //Respawn
            //Respawn
            StartCoroutine(BasketRespawn(respawnTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            GameManager.instance.applePanel.SetActive(true); //Read the game manager link
            print("Triggered");
            txtApples = GameManager.instance.txtApples;

            //Give the player a number of apples (which may not be enough to kill the boss)
            GameManager.instance.apples = Random.Range(3, 5);

            

            //Update the panel
            txtApples.text = GameManager.instance.apples.ToString();


            //Make basket invisible
            //Find the doctor
            Vector3 doctor = transform.parent.position;
            //New basket position
            float y = doctor.y + 20; //Vertical pos
            float x = doctor.x - Random.Range(2, 25);

            transform.position = new Vector3(x, y, 0);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
           

           

        }
    }

    IEnumerator BasketRespawn(int respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        print(basketRespawnning);
       

        
        //Launch the basket
        AudioManager.instance.Play("Can1", 0.2f);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        print("Sprite renderer on");
    }


}