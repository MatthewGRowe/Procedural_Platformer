using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHydrant : MonoBehaviour
{
    [SerializeField]
    GameObject WaterJet;

    private SpriteRenderer myImage;
    private bool waterIsOnDisplay = false;
    private float springForce = 20f; //Force applied to player
    private bool donkSound = true;

    private void Start()
    {
        myImage = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.BULLET_TAG && waterIsOnDisplay == false)
        {
            waterIsOnDisplay = true;

            //All player attacks use a bullet tag
            AudioManager.instance.Play(MyTags.BULLET_TAG);

            //Hide the sprite
            myImage.enabled = false;

            //Play the waterjet
            WaterJet.SetActive(true);
            AudioManager.instance.Play("Waterfall", 1.8f);
        }

        else if (collision.tag == MyTags.PLAYER_TAG && waterIsOnDisplay)  //Happens later after player has attacked the hydrant
        {
            // Launch player straight up (could add directional logic if you want chaos)
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x, 0); // Cancel existing Y velocity
            rb.AddForce(Vector2.up * springForce, ForceMode2D.Impulse);
            AudioManager.instance.Play(MyTags.SOUND_BOING);



        }
        else if (collision.tag == MyTags.PLAYER_TAG && waterIsOnDisplay == false && donkSound)
        {
            donkSound = false;
            AudioManager.instance.Play(MyTags.SOUND_DONK);
            StartCoroutine(DonkAgain());
        }
    }

    IEnumerator DonkAgain()
    {
        yield return new WaitForSeconds(0.6f);
        donkSound = true;
    }
}
