using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MadDoctor : MonoBehaviour
{
    //Take damage
    //Fade away when killed
    public bool iAmDead = false; //Set by boss health script
    public GameObject trophy;  //To be instaniated when Doc Dies

    //Used for flashing the doctor when he is hit
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    Vector3 trophyPosition;

    private void Start()
    {
        trophyPosition = transform.position;  //Where the trophy should pop up from
        trophyPosition.x += 5;  //Slightly to the right
        trophyPosition.y -= 35;  //Down a bit
    }

    public void TryLungeForward()
    {
        float chance = Random.value;
        if (chance >= 0.6f) return; // 40% chance to stay still

        float lungeDistance = Random.Range(1f, 3f);
        float facing = -1f; // force "forward" to mean left

        //Reset if required
        if (Mathf.Abs(transform.position.x - trophyPosition.x) > 25f)
        {
            Vector3 reset = transform.position;
            reset.x = trophyPosition.x;
            transform.position = reset;

            //Debug.Log("Mad Doctor wandered too far. Resetting to base.");
        }
        else
        {

            if (Random.value < 0.4f)
            {
                facing = 1f; // 60% chance to move right instead (backward-ish)
            }

            Vector3 target = transform.position + new Vector3(lungeDistance * facing, 0, 0);

            // Clamp so they don't drift too far from their original position
            float maxDistance = 25f;
            float offset = target.x - trophyPosition.x;

            if (Mathf.Abs(offset) > maxDistance)
            {
                target.x = trophyPosition.x + (maxDistance * Mathf.Sign(offset));
            }

            transform.position = target;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.tag =="Apple")
        {
            // Get the first contact point from the collision (where the apple touched this object)
            ContactPoint2D contact = collision.GetContact(0);

            // Extract the exact world position of the collision point
            Vector2 contactPoint = contact.point;

            // Get a reference to this object's CapsuleCollider2D component
            CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();

            // Get the world-space bounding box (top, bottom, sides) of the capsule collider
            Bounds bounds = capsule.bounds;

            // Calculate how far up the object was hit (0 = bottom, 1 = top)
            // This normalizes the hit position between the bottom and top Y bounds of the collider
            float relativeY = Mathf.InverseLerp(bounds.min.y, bounds.max.y, contactPoint.y);


            float damage = 0f;
            //string hitZone = "";

            if (relativeY >= 0.66f)
            {
                // Headshot
                damage = 20f;
                int newScore = (int)damage * 4; 
                GameManager.instance.score += newScore;
                GameManager.instance.InfoTextDisplay("Head shot! " + newScore, 2);

            }
            else if (relativeY >= 0.33f)
            {
                // Torso
                damage = 10f;
                int newScore = (int)damage * 2;
                GameManager.instance.score += newScore;
                GameManager.instance.InfoTextDisplay("Body shot! " + newScore, 2);

            }
            else
            {
                // Legs
                damage = 5f;
                int newScore = (int)damage;
                GameManager.instance.score += newScore;
                GameManager.instance.InfoTextDisplay("Leg shot :( " + newScore, 2);

            }

            Destroy(collision.gameObject); //Destroy the apple

            GetComponentInChildren<BossHealth>().TakeDamage(damage);
            TriggerHurtFlash();

           
        }

      

    }

    bool runOnce = false;

    private void Update()
    {
        if (iAmDead && runOnce == false)
        {
            runOnce = true;
            print("Boss Dead!");
            GameManager.instance.ResetAmbience();
            GameManager.instance.bossAttack = false;
            AudioManager.instance.Play("PagingDoctor");
            GetComponent<Animator>().Play("Shuffle");
            //Turn around
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            //Pause and run
            StartCoroutine(DoctorRunAway());


        }

    }

    IEnumerator DoctorRunAway()
    {
        
        //Make the doctor slide off the right

        yield return new WaitForSeconds(3f); // Wait for page sfx or drama to finish

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = 1f;
            rb.drag = 0;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;


            rb.velocity = new Vector2(4f, 0f); // slide to the right
        }

        yield return new WaitForSeconds(5f);

        Destroy(gameObject); // Farewell, you evil medic

        Victory();


    }

    private void Victory()
    {

        //Play tune
        AudioManager.instance.StopAllSounds();
        int tune = Random.Range(1, 5);  //Select 1..4
        switch (tune)
        {
            case 1:
                AudioManager.instance.Play("Victory1");
                break;
            case 2:
                AudioManager.instance.Play("Victory2");
                break;
            case 3:
                AudioManager.instance.Play("Victory3");
                break;
            case 4:
                AudioManager.instance.Play("Victory4");
                break;
        }



        //Present cup (which will end the game)
        
        GameObject newCheckpoint = Instantiate(trophy, trophyPosition, Quaternion.identity);
    }

    public void TriggerHurtFlash()
    {
        StartCoroutine(HurtFlashAllParts());
    }

    private IEnumerator HurtFlashAllParts()
    {
        // Get all SpriteRenderers under this object
        SpriteRenderer[] parts = GetComponentsInChildren<SpriteRenderer>();

        // Store original colors
        Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
        foreach (var sr in parts)
        {
            originalColors[sr] = sr.color;
            sr.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        // Reset all colors
        foreach (var sr in parts)
        {
            if (sr != null) sr.color = originalColors[sr];
        }

        yield return new WaitForSeconds(flashDuration);
        foreach (var sr in parts)
        {
            originalColors[sr] = sr.color;
            sr.color = flashColor;
        }
        yield return new WaitForSeconds(flashDuration);

        // Reset all colors
        foreach (var sr in parts)
        {
            if (sr != null) sr.color = originalColors[sr];
        }
    }


}
