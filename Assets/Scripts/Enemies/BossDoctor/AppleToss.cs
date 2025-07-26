using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleToss : MonoBehaviour
{
    //There are a lot of players in this game therefore this is a standalone object
    //It will find the player and guess a position to throw from, this sounds like a good 
    //idea now but we will see

    public GameObject apple;  //Apple to throw
    private Vector3 instantiationPoint; //May need to swap this to an actual tranform
    private Vector3 offset = new Vector3(15, 0, 0);

    private float horizontalSpeed = 9f;  //These enable you to control the trajectory of the throw.
    private float verticalSpeed = 1.5f;
    private bool canShoot = true;



    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.apples > 0)
        {
            if (Input.GetKeyDown(KeyCode.B) && canShoot)
            {
                canShoot = false; //Short delay to prevent rapid firing
                StartCoroutine(EnableShoot());
            }
        }

    }

    IEnumerator EnableShoot()
    {
        GameObject player = GameObject.FindWithTag(MyTags.PLAYER_TAG);

        // Spawn in front of and slightly above the player
        float facing = Mathf.Sign(player.transform.localScale.x); // 1 for right, -1 for left

        // Special-case the angry fruit-tossing blob known as Gordo
        // Gordo is the only prefab that faces left by default.
        // Reverse his apple toss direction so he doesn’t throw from his backside.
        if (player.name.Contains("Gordo")) // or == "Gordo(Clone)" if it's instantiated
        {
            facing *= -1f;
        }



        Vector3 spawnOffset = new Vector3(0.75f * facing, 0.5f, 0); // tweak these values as needed
        Vector3 spawnPoint = player.transform.position + spawnOffset;

        // Instantiate apple
        GameObject currentApple = Instantiate(apple, spawnPoint, Quaternion.identity);

        //Grunt
        AudioManager.instance.Play("HeavyThrow", Random.Range(1.6f, 3f));
        // Throw with velocity
        Vector2 throwVelocity = new Vector2(horizontalSpeed * facing, verticalSpeed);
        Rigidbody2D rb = currentApple.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = throwVelocity;
        }

        // Add random spin for flair
        currentApple.transform.eulerAngles = new Vector3(0, 0, Random.Range(-50f, 50f));

        // Update apple count
        GameManager.instance.apples--;
        
        if (GameManager.instance.apples ==0)
        {
            //Enable fruit basket to respawn, search siblings (parent -> children)
            gameObject.transform.parent.GetComponentInChildren<AppleBasket>().basketRespawnning = false;
        }

                GameManager.instance.txtApples.text = GameManager.instance.apples.ToString();

        yield return new WaitForSeconds(0.5f);
        canShoot = true;


    }


}


